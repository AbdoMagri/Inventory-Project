import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Auth } from "../../context/AuthContext";
import { login, createAccount } from "../../api/authApi";
import Input from "../../components/Input";
import Button from "../../components/Button";
import "./login.css";

const emptyForm = {
  accountName: "",
  fullName: "",
  username: "",
  password: "",
  confirmPassword: "",
};

export default function LoginPage() {
  const [mode, setMode] = useState("login"); // "login" | "register"
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const { signIn } = Auth();
  const navigate = useNavigate();

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const switchMode = (m) => {
    setMode(m);
    setError("");
    setForm(emptyForm);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (mode === "register" && form.password !== form.confirmPassword) {
      setError("Passwords do not match");
      return;
    }

    setLoading(true);
    try {
      if (mode === "register") {
        await createAccount({
          accountName: form.accountName,
          fullName: form.fullName,
          username: form.username,
          password: form.password,
        });
      }
      const user = await login(form.username, form.password);
      signIn(user);
      navigate("/");
    } catch (err) {
      setError(err.message || "Something went wrong");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h2>Inventory System</h2>

        <div className="login-tabs">
          <button
            type="button"
            className={mode === "login" ? "tab active" : "tab"}
            onClick={() => switchMode("login")}
          >
            Login
          </button>
          <button
            type="button"
            className={mode === "register" ? "tab active" : "tab"}
            onClick={() => switchMode("register")}
          >
            Create account
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          {mode === "register" && (
            <>
              <Input
                label="Account / Store name"
                name="accountName"
                value={form.accountName}
                onChange={handleChange}
                required
              />
              <Input
                label="Full name"
                name="fullName"
                value={form.fullName}
                onChange={handleChange}
                required
              />
            </>
          )}

          <Input
            label="Username"
            name="username"
            value={form.username}
            onChange={handleChange}
            required
          />
          <Input
            label="Password"
            type="password"
            name="password"
            value={form.password}
            onChange={handleChange}
            required
          />

          {mode === "register" && (
            <Input
              label="Confirm password"
              type="password"
              name="confirmPassword"
              value={form.confirmPassword}
              onChange={handleChange}
              required
            />
          )}

          {error && <p className="login-error">{error}</p>}

          <Button type="submit" disabled={loading} style={{ width: "100%" }}>
            {loading
              ? "Please wait..."
              : mode === "login"
              ? "Login"
              : "Create account"}
          </Button>
        </form>
      </div>
    </div>
  );
}