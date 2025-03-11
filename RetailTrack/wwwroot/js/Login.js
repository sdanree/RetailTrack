let AUTH0_DOMAIN, CLIENT_ID, AUDIENCE;
const REDIRECT_URI = `${window.location.origin}/home.html`; // Redirige a Home después del login

// 📌 Cargar la configuración desde config.json
async function loadConfig() {
    try {
        const response = await fetch('/config.json');
        const config = await response.json();
        AUTH0_DOMAIN = config.auth0Domain;
        CLIENT_ID = config.clientId;
        AUDIENCE = config.audience;
    } catch (error) {
        console.error("❌ Error cargando la configuración:", error);
    }
}

// 📌 Llamar a loadConfig antes de ejecutar cualquier función que necesite estos valores
loadConfig().then(() => {
    document.getElementById("login").addEventListener("click", login);
    document.getElementById("logout").addEventListener("click", logout);
    checkAuth();
});

// 📌 Función de login con Auth0
async function login() {
    if (!AUTH0_DOMAIN || !CLIENT_ID || !AUDIENCE) {
        console.error("❌ Configuración no cargada correctamente.");
        return;
    }

    const authUrl = `https://${AUTH0_DOMAIN}/authorize?` +
        `client_id=${CLIENT_ID}&` +
        `redirect_uri=${REDIRECT_URI}&` +
        `response_type=code&` +
        `scope=openid profile email&` +
        `audience=${AUDIENCE}`;

    window.location.href = authUrl;
}

// 📌 Intercambio del código por un token de acceso
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
            console.error("❌ No se pudo obtener el token de acceso:", data);
        }
    } catch (error) {
        console.error("❌ Error al intercambiar código por token:", error);
    }
}

// 📌 Verificar autenticación al cargar la página
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

// 📌 Logout: eliminar token y redirigir
function logout() {
    localStorage.removeItem("access_token");
    window.location.href = "/Account/Login"; // Redirige a la página de login
}
