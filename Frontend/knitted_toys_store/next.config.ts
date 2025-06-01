import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  eslint: {
    ignoreDuringBuilds: true,
  },
  env: {
    //NEXT_PUBLIC_DEV_API_BASE_URL: 'http://localhost/api',
    NEXT_PUBLIC_DEV_API_BASE_URL: 'http://nginx',
  },
};

export default nextConfig;
