import { render, screen, fireEvent } from '@testing-library/react';
import App from './App';

describe('App', () => {
  it('renders Vite + React heading', () => {
    render(<App />);
    const heading = screen.getByText(/Vite \+ React/i);
    expect(heading).toBeInTheDocument();
  });

  it('increments count when button is clicked', () => {
    render(<App />);
    const button = screen.getByRole('button', { name: /count is 0/i });
    
    fireEvent.click(button);
    
    expect(screen.getByText(/count is 1/i)).toBeInTheDocument();
  });

  it('renders both logos', () => {
    render(<App />);
    const viteLogo = screen.getByAltText(/Vite logo/i);
    const reactLogo = screen.getByAltText(/React logo/i);
    
    expect(viteLogo).toBeInTheDocument();
    expect(reactLogo).toBeInTheDocument();
  });
});
