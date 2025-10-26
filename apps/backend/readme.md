# granum demo app backend

## design decisions
- three-tier architecture (controller, service, repository)
- feature organization (by feature, not by layer)
- use openapi spec to generate controllers and models
- try out new Scalar UI instead of Swagger UI

## implementation plan

- [X] set up project structure using three-tier architecture and feature organization
- [X] implement a database schema using architecture plans
- [X] implement a repository
- [X] implement a controller
- [x] implement a service
- [x] define fully generic UserController class
- [x] setup exception handling middleware for restful repsonses with thin controllers
- [x] implement unit tests for each layer
  (up until this point AI tools had been used exclusively for reference. The code written is my own based on the architecture plans and patterns I have used in the past.)
- [x] implement controller integration tests (gherkin, taiko)
- [x] fix issues found by controller integration tests
- [ ] implement database integration tests
- [ ] come back to openapi page
- [ ] use source-generated DI with attributes and Microsoft.Extensions.DependencyInjection.SourceGeneration
- [ ] define an openapi spec to generate the rest of the controllers
- [ ] use AI to spit out the rest of the service layers after we have one defined
- [ ] switch to SQLite
- [ ] switch to EF migrations
- [ ] inject logger into service and controller layers
- [ ] set up docker for local development
- [ ] set up CI/CD pipeline

after you have a UI going... 
- [ ] implement DTOs and mappers (automap?)
- [ ] implement end-to-end-testing

nice to haves
- [ ] setup editorconfig