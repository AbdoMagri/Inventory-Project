import { Outlet } from "react-router-dom";

export default function MainLayout() {
  return (
    <div>
      <h3>Main layout</h3>
      <Outlet />
    </div>
  );
}