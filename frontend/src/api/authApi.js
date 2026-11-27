import { api } from "./client";

export const login = (username, password) =>
  api.post("/Auth/login", { username, password });

export const createAccount = (data) =>
  api.post("/Accounts/Register", data);