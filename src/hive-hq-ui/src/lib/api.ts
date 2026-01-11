// hive-hq-ui/src/lib/api.ts
const BASE_URL = "https://localhost:7250/api"; // Check your API's actual port


// Helper to get token from local storage
const getAuthHeader = (): Record<string, string> => {
  const token = localStorage.getItem("token");
  return token ? { Authorization: `Bearer ${token}` } : {};
};

export async function getInventory() {
  const response = await fetch(`${BASE_URL}/Inventory`, {
    headers: {
      ...getAuthHeader(),
      "Content-Type": "application/json",
    },
  });

  if (response.status === 401) {
    // Optional: Redirect to login if unauthorized
    window.location.href = "/login";
  }

  if (!response.ok) throw new Error("Failed to fetch inventory");
  return response.json();
}

// Add a Login function
export async function login(email: string, password: string) {
  const response = await fetch("https://localhost:7250/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });

  if (!response.ok) throw new Error("Login failed");

  const data = await response.json();
  localStorage.setItem("token", data.accessToken); // Store the JWT
  return data;
}
export async function getDashboardStats() {
  const response = await fetch(`${BASE_URL}/statistics/daily-summary`, {
    next: { revalidate: 300 } // Cache for 5 minutes (matches your Redis logic!)
  });

  if (!response.ok) throw new Error("Failed to fetch statistics");
  return response.json();
}
