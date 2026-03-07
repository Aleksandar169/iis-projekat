import { Link } from '@tanstack/react-router';

export function Header() {
  return (
    <header className="app-header">
      <div className="container nav">
        <div>
          <h1 className="site-title">IIS Frontend</h1>
          <p className="site-subtitle">Kupovina karata za Formulu 1</p>
        </div>

        <nav className="nav-links">
          <Link to="/" className="nav-link">
            Početna
          </Link>
          <Link to="/ticket/new" className="nav-link">
            Kupovina karte
          </Link>
          <Link to="/ticket/view" className="nav-link">
            Pregled / izmena karte
          </Link>
          <Link to="/administration" className="nav-link">
            Administracija
          </Link>
        </nav>
      </div>
    </header>
  );
}