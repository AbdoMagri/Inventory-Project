import { Navigate } from "react-router-dom";
import { Auth } from "../context/AuthContext";

export default function ProtectedRoute({ children }) {
  const { user } = Auth();
  return user ? children : <Navigate to="/login" replace />;
}