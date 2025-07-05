import axios from "axios";

export const adminFileAPI = axios.create({
    baseURL: "/api", // <-- относительный путь!
    withCredentials: true,
    responseType: 'blob',
    headers: {
        "Content-Type": "application/json",
    },
});

adminFileAPI.interceptors.request.use((config) => {
    const token = localStorage.getItem("admin_token");
    if(token){
        config.headers.Authorization = `Basic ${token}`;
    }
    
    return config;
});

export default adminFileAPI;


// import axios from "axios";

// export const adminFileAPI = axios.create({
//     baseURL: process.env.NEXT_PUBLIC_DEV_API_BASE_URL,
//     withCredentials: true,
//     responseType: 'blob',
//     headers: {
//         "Content-Type": "application/json",
//     },
// });

// adminFileAPI.interceptors.request.use((config) => {
//     const token = localStorage.getItem("admin_token");
//     if(token){
//         config.headers.Authorization = `Basic ${token}`;
//     }
    
//     return config;
// });

// export default adminFileAPI;

