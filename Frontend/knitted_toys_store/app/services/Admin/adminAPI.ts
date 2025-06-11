import axios from "axios";

export const adminAPI = axios.create({
    baseURL: "/api", // <-- относительный путь!
    withCredentials: true,
    headers: {
        "Content-Type": "application/json",
    },
});

adminAPI.interceptors.request.use((config) => {
    const token = localStorage.getItem("admin_token");
    if (token) {
        config.headers.Authorization = `Basic ${token}`;
    }

    return config;
});

export default adminAPI;