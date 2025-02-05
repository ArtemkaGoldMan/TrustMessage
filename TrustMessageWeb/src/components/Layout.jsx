import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { checkAuth, logout } from '../services/authService';
import { useEffect } from 'react';

export default function Layout() {
  const navigate = useNavigate();

  useEffect(() => {
    const verifyAuth = async () => {
      try {
        await checkAuth();
      } catch (err) {
        navigate('/');
      }
    };

    verifyAuth();
  }, [navigate]);

  const handleLogout = async () => {
    try {
      await logout();
      window.location.href = '/'; 
    } catch (err) {
      console.error('Logout failed:', err);
    }
  };

  return (
    <div className="App">
      <div className="layout">
        <nav>
          <NavLink 
            to="/create" 
            className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}
          >
            Create Message
          </NavLink>
          <NavLink 
            to="/general" 
            className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}
          >
            General Messages
          </NavLink>
          <NavLink 
            to="/personal" 
            className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}
          >
            My Messages
          </NavLink>
          <NavLink 
            to="/change-password" 
            className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}
          >
            Change Password
          </NavLink>
          <button onClick={handleLogout} className="logout-button">
            Logout
          </button>
        </nav>
        <div className="content">
          <Outlet />
        </div>
      </div>
    </div>
  );
} 