# CI/CD Implementation Plan - granum-demo

## Overview

This document describes the comprehensive CI/CD pipeline implementation for the granum-demo monorepo using GitHub Actions. The pipeline provides parallel test execution, code coverage reporting, containerization, and automated deployment.

## Architecture

### Technology Stack

- **Backend**: .NET 9.0 with NUnit testing framework
- **Frontend**: React 19 with Vite and Jest testing framework
- **Database**: PostgreSQL 17 (via TestContainers for integration tests)
- **Container Registry**: GitHub Container Registry (ghcr.io)
- **Code Coverage**: Coverlet for backend, Jest for frontend

### Pipeline Components

1. **Configuration Files** (`.github/config/`)
   - `versions.yml` - Centralized version management for tools and dependencies
   - `test-settings.yml` - Test execution configuration and timeouts

2. **Composite Actions** (`.github/actions/`)
   - `load-config` - Load and parse configuration YAML files
   - `setup-backend` - Setup .NET SDK and restore dependencies with caching
   - `setup-frontend` - Setup Node.js and install npm packages with caching

3. **Reusable Workflows** (`.github/workflows/`)
   - `backend-test.yml` - Backend unit and integration test execution
   - `frontend-test.yml` - Frontend unit test execution

4. **Main Workflows**
   - `ci.yml` - Main CI pipeline orchestration
   - `system-test.yml` - Manual end-to-end smoke testing

## CI Pipeline Flow

### Main CI Pipeline (`ci.yml`)

```
┌─────────────────────────────────────────────────────────┐
│                   CI Pipeline Trigger                    │
│          (push to main or pull request)                  │
└─────────────────────────────────────────────────────────┘
                          │
         ┌────────────────┼────────────────┐
         │                │                │
         ▼                ▼                ▼
┌────────────────┐ ┌──────────────┐ ┌────────────────┐
│  Backend Unit  │ │   Backend    │ │   Frontend     │
│     Tests      │ │ Integration  │ │     Tests      │
│                │ │    Tests     │ │                │
│  (Parallel)    │ │  (Parallel)  │ │  (Parallel)    │
└────────────────┘ └──────────────┘ └────────────────┘
         │                │                │
         └────────────────┼────────────────┘
                          ▼
                ┌──────────────────┐
                │  Quality Gate    │
                │  (All Pass?)     │
                └──────────────────┘
                          │
         ┌────────────────┼────────────────┐
         │                │                │
         ▼                ▼                ▼
┌────────────────┐ ┌──────────────┐ ┌────────────────┐
│   Coverage     │ │    Build     │ │     Build      │
│   Reporting    │ │   Backend    │ │   Frontend     │
│  (PR only)     │ │   Image      │ │    Image       │
│                │ │ (main only)  │ │  (main only)   │
└────────────────┘ └──────────────┘ └────────────────┘
```

### Test Execution Strategy

All tests run in **parallel** for maximum efficiency:

1. **Backend Unit Tests**
   - Project: `Granum.Tests`
   - Uses in-memory database
   - Timeout: 10 minutes
   - Coverage: OpenCover format

2. **Backend Integration Tests**
   - Project: `Granum.IntegrationTests`
   - Uses TestContainers with PostgreSQL 17
   - WebApplicationFactory for API testing
   - Timeout: 15 minutes
   - Coverage: OpenCover format

3. **Frontend Unit Tests**
   - Framework: Jest with React Testing Library
   - Timeout: 10 minutes
   - Coverage: LCOV and Cobertura formats

### Quality Gate

The quality gate ensures all tests pass before:
- Building Docker images (on main branch)
- Generating coverage reports

If any test suite fails, the pipeline stops and subsequent jobs are skipped.

### Coverage Reporting

**Trigger**: Pull requests only

**Process**:
1. Download coverage artifacts from all test jobs
2. Generate unified coverage summary
3. Post summary to GitHub step summary
4. Non-blocking - does not fail the build

**Formats**:
- Backend: OpenCover XML
- Frontend: LCOV, Cobertura, JSON summary

### Container Builds

**Trigger**: Push to main branch only

**Images Built**:
1. **Backend Image** (`ghcr.io/{owner}/granum-demo/backend`)
   - Multi-stage build (build → publish → runtime)
   - Base: mcr.microsoft.com/dotnet/aspnet:9.0
   - Includes health check endpoint
   - Port: 5134

2. **Frontend Image** (`ghcr.io/{owner}/granum-demo/frontend`)
   - Multi-stage build (build → runtime with nginx)
   - Base: nginx:alpine
   - Includes custom nginx configuration
   - Includes health check endpoint
   - Port: 80

**Tags**:
- `latest` - Latest build from main branch
- `main-{sha}` - Specific commit SHA
- `main` - Branch name tag

**Features**:
- Layer caching via GitHub Actions cache
- BuildKit for optimized builds
- Automatic push to GitHub Container Registry

## System Tests

### Manual Workflow (`system-test.yml`)

**Trigger**: Manual dispatch via GitHub UI

**Inputs**:
- `backend_image` - Backend Docker image tag (default: latest)
- `frontend_image` - Frontend Docker image tag (default: latest)

**Test Flow**:
1. Start PostgreSQL container (postgres:17-alpine)
2. Build and start backend service
3. Build and start frontend service
4. Wait for services to be ready (health checks)
5. Run smoke tests:
   - Backend API availability
   - OpenAPI endpoint validation
   - Frontend availability
   - Frontend content validation

**Smoke Tests**:
```bash
# Backend API health check
GET http://localhost:5134/scalar/v1 → 200 OK

# OpenAPI endpoint
GET http://localhost:5134/openapi/v1.json → Valid JSON response

# Frontend availability
GET http://localhost:5173 → 200 OK

# Frontend content
GET http://localhost:5173 → Contains "Vite + React"
```

## Configuration Management

### Version Centralization (`versions.yml`)

All tool versions are centralized for easy maintenance:

```yaml
dotnet:
  sdk_version: '9.0.x'
node:
  version: '20.x'
docker:
  postgres_version: '17-alpine'
actions:
  checkout_version: 'v4'
  setup_dotnet_version: 'v4'
  setup_node_version: 'v4'
  # ... etc
```

### Test Settings (`test-settings.yml`)

Test execution parameters:

```yaml
backend:
  unit_tests:
    project: 'Granum.Tests'
    timeout_minutes: 10
    coverage: true
  integration_tests:
    project: 'Granum.IntegrationTests'
    timeout_minutes: 15
    coverage: true
    requires_docker: true

frontend:
  unit_tests:
    timeout_minutes: 10
    coverage: true

system_tests:
  timeout_minutes: 15
  backend_url: 'http://localhost:5134'
  frontend_url: 'http://localhost:5173'
```

## Composite Actions

### setup-backend

**Purpose**: Setup .NET SDK and restore dependencies

**Features**:
- Installs .NET SDK (version from input or default)
- Caches NuGet packages (~/.nuget/packages)
- Restores solution dependencies
- Displays SDK version information

**Cache Key**: Based on `packages.lock.json` and `*.csproj` files

### setup-frontend

**Purpose**: Setup Node.js and install dependencies

**Features**:
- Installs Node.js (version from input or default)
- Caches npm packages (uses package-lock.json)
- Runs `npm ci` for deterministic installs
- Displays Node.js and npm versions

**Cache Key**: Based on `package-lock.json`

### load-config

**Purpose**: Load and parse YAML configuration files

**Features**:
- Installs `yq` utility if not present
- Converts YAML to JSON
- Outputs configuration as multiline string

**Usage**:
```yaml
- uses: ./.github/actions/load-config
  with:
    config-type: 'versions'
```

## Docker Configuration

### Backend Dockerfile

**Multi-stage build**:
1. **Build stage**: Restore and build the application
2. **Publish stage**: Publish optimized release build
3. **Runtime stage**: Minimal runtime image with ASP.NET Core

**Key features**:
- Uses official Microsoft .NET images
- Includes curl for health checks
- Health check on port 5134
- Environment: Production by default
- Non-root user execution (inherited from base image)

### Frontend Dockerfile

**Multi-stage build**:
1. **Build stage**: Install dependencies and build with Vite
2. **Runtime stage**: Serve with nginx

**Key features**:
- Uses official Node and nginx Alpine images
- Custom nginx configuration for SPA routing
- Gzip compression enabled
- Security headers configured
- Health check endpoint
- Static asset caching

### Nginx Configuration

**Features**:
- SPA routing (fallback to index.html)
- Gzip compression for text assets
- Long-term caching for static assets (1 year)
- Security headers (X-Frame-Options, X-Content-Type-Options, X-XSS-Protection)
- Health check endpoint at `/health`

## Caching Strategy

### NuGet Package Cache

**Location**: `~/.nuget/packages`

**Key**: `${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json', '**/*.csproj') }}`

**Benefit**: Faster dependency restoration (typically 30-60 seconds saved)

### npm Package Cache

**Location**: Managed by `setup-node` action

**Key**: Based on `package-lock.json`

**Benefit**: Faster dependency installation (typically 15-30 seconds saved)

### Docker Layer Cache

**Type**: GitHub Actions cache

**Benefit**: Faster image builds by reusing layers

## Optimization Features

1. **Parallel Execution**: All test suites run simultaneously
2. **Concurrency Control**: Cancels in-progress runs when new commits pushed
3. **Conditional Jobs**: 
   - Coverage reporting only on PRs
   - Container builds only on main branch
4. **Artifact Retention**: 7 days for test results and coverage
5. **Continue on Error**: Coverage download failures don't fail the build

## Security Considerations

1. **GitHub Container Registry**: Uses `GITHUB_TOKEN` for authentication
2. **Image Scanning**: Can be integrated with Trivy or other scanners (future enhancement)
3. **Secrets Management**: No hardcoded credentials
4. **Non-blocking Coverage**: Coverage reports don't block deployment
5. **HTTPS**: All external communications use HTTPS

## Monitoring and Observability

### Test Results
- Uploaded as artifacts (retention: 7 days)
- TRX format for backend tests
- JUnit format for frontend tests (configurable)

### Coverage Reports
- Uploaded as artifacts (retention: 7 days)
- Multiple formats for compatibility
- Posted to PR as summary

### Build Logs
- Available in GitHub Actions UI
- Includes version information
- Detailed error messages on failure

### Container Images
- Tagged with commit SHA for traceability
- Listed in GitHub Packages
- Includes metadata labels

## Future Enhancements

1. **E2E Tests**: Add Playwright/Cypress tests
2. **Performance Testing**: Load testing for API endpoints
3. **Security Scanning**: Integrate CodeQL, Trivy, or Snyk
4. **Deployment**: Add staging/production deployment workflows
5. **Notifications**: Slack/email notifications on failures
6. **Metrics**: Collect and report test execution metrics
7. **Branch Protection**: Require status checks before merge

## Troubleshooting

### Common Issues

1. **Test Failures**
   - Check test logs in artifacts
   - Verify TestContainers has Docker access
   - Ensure PostgreSQL image is accessible

2. **Build Failures**
   - Check NuGet/npm connectivity
   - Verify .NET SDK/Node.js versions
   - Check for breaking dependency changes

3. **Docker Build Issues**
   - Verify Dockerfile syntax
   - Check for missing files (.dockerignore)
   - Ensure base images are accessible

4. **Cache Issues**
   - Clear cache via GitHub UI if corrupted
   - Verify cache key patterns

## Maintenance

### Updating Versions

1. Edit `.github/config/versions.yml`
2. Update version numbers
3. Test in a PR before merging

### Updating Test Configuration

1. Edit `.github/config/test-settings.yml`
2. Adjust timeouts or settings
3. Validate in a test run

### Updating Workflows

1. Edit workflow files in `.github/workflows/`
2. Use `act` tool for local testing (optional)
3. Create PR and verify workflow runs

## Conclusion

This CI/CD implementation provides:
- ✅ Fast, parallel test execution
- ✅ Comprehensive code coverage reporting
- ✅ Automated container builds
- ✅ Flexible system testing
- ✅ Maintainable, reusable components
- ✅ Production-ready containerization

The pipeline is designed for scalability and can be extended with additional workflows, tests, and deployment targets as needed.
