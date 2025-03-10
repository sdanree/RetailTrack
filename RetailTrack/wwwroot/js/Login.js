const AUTH0_DOMAIN = "dev-tb7nfy6zdhb2maz5.us.auth0.com"; 
const CLIENT_ID = "26R24q6lvSsajcU6mexK6NLUZ0PsTqfJ"; 
const AUDIENCE = "https://dev-tb7nfy6zdhb2maz5.us.auth0.com/api/v2/";
const REDIRECT_URI = `${window.location.origin}/home.html`; // Redirige a Home después del login

async function login() {
    const authUrl = `https://${AUTH0_DOMAIN}/authorize?` +
        `client_id=${CLIENT_ID}&` +
        `redirect_uri=${REDIRECT_URI}&` + 
        `response_type=code&` +  // Usa Authorization Code Flow
        `scope=openid profile email&` +
        `audience=${AUDIENCE}`;

    window.location.href = authUrl;
}

async function fetchAccessToken(code) {
    try {
        const response = await fetch(`https://${AUTH0_DOMAIN}/oauth/token`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                grant_type: "authorization_code",
                client_id: CLIENT_ID,
                code: code,
                redirect_uri: REDIRECT_URI
            })
        });

        const data = await response.json();

        if (data.access_token) {
            localStorage.setItem("access_token", data.access_token);
            window.location.href = "/home.html"; // Redirige si el token es válido
        } else {
            console.error("No se pudo obtener el token de acceso:", data);
        }
    } catch (error) {
        console.error("Error al intercambiar código por token:", error);
    }
}

function checkAuth() {
    const params = new URLSearchParams(window.location.search);
    const code = params.get("code");  // Obtener el código de la URL

    if (code) {
        fetchAccessToken(code); // Intercambia el código por un token de acceso
    } else {
        const storedToken = localStorage.getItem("access_token");
        if (storedToken) {
            window.location.href = "/home.html"; // Ya autenticado, redirige a Home
        }
    }
}

function logout() {
    localStorage.removeItem("access_token");
    window.location.href = "/"; // Redirige a la página de login
}

document.getElementById("login").addEventListener("click", login);
document.getElementById("logout").addEventListener("click", logout);

checkAuth();
