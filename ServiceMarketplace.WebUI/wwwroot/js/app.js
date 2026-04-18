async function login() {
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    const response = await fetch("https://localhost:44343/api/Auth/login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            email: email,
            password: password
        })
    });

    if (!response.ok) {
        alert("Login failed ❌");
        return;
    }

    const token = await response.text();

    // 🔥 حفظ التوكن
    localStorage.setItem("token", token);

    // 🔥 قراءة الـ payload من JWT
    const payload = JSON.parse(atob(token.split('.')[1]));

    const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    // 🔥 تحويل حسب الدور
    if (role === "Customer") {
        window.location.href = "/Customer/Index";
    } else if (role === "Provider") {
        window.location.href = "/Provider/Index";
    } else if (role === "Admin") {
        window.location.href = "/Admin/Index";
    }
}

async function register() {
    const fullName = document.getElementById("fullName").value;
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    const response = await fetch("https://localhost:44343/api/Auth/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            fullName: fullName,
            email: email,
            password: password
        })
    });

    if (!response.ok) {
        alert("Registration failed ❌");
        return;
    }

    const token = await response.text();

    // token
    localStorage.setItem("token", token);

    const payload = JSON.parse(atob(token.split('.')[1]));
    const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    // role
    if (role === "Customer") {
        window.location.href = "/Customer/Index";
    } else if (role === "Provider") {
        window.location.href = "/Provider/Index";
    } else if (role === "Admin") {
        window.location.href = "/Admin/Index";
    }
}

async function createRequest() {

    const title = document.getElementById("title").value.trim();
    const description = document.getElementById("description").value.trim();
    const lat = document.getElementById("lat").value;
    const lng = document.getElementById("lng").value;

    // 🔥 Validation
    if (!title || !lat || !lng) {
        alert("❌ Title, Latitude, Longitude are required");
        return;
    }

    if (isNaN(lat) || isNaN(lng)) {
        alert("❌ Latitude & Longitude must be numbers");
        return;
    }

    const token = localStorage.getItem("token");

    const response = await fetch("https://localhost:44343/api/Requests", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify({
            title: title,
            description: description, // optional ✔
            latitude: parseFloat(lat),
            longitude: parseFloat(lng)
        })
    });

    if (!response.ok) {
        const error = await response.text();
        alert(error);
        return;
    }

    alert("✅ Request created");

    // reset
    document.getElementById("title").value = "";
    document.getElementById("description").value = "";
    document.getElementById("lat").value = "";
    document.getElementById("lng").value = "";
}

async function loadMyRequests() {
    const token = localStorage.getItem("token");

    const response = await fetch("https://localhost:44343/api/Requests", {
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        }
    });

    if (!response.ok) {
        alert("Failed to load requests");
        return;
    }

    const data = await response.json();

    const list = document.getElementById("requestsList");
    list.innerHTML = "";

    data.forEach(req => {
        const li = document.createElement("li");

        li.innerHTML = `
        <strong>${req.title}</strong><br/>
        <small>${req.description}</small>
    `;

        list.appendChild(li);
    });
}

async function loadNearbyRequests() {
    const token = localStorage.getItem("token");

    const lat = document.getElementById("providerLat").value;
    const lng = document.getElementById("providerLng").value;

    const response = await fetch(`https://localhost:44343/api/Requests?lat=${lat}&lng=${lng}`, {
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        }
    });

    if (!response.ok) {
        alert("Failed to load requests ❌");
        return;
    }

    const data = await response.json();

    const list = document.getElementById("providerRequests");
    list.innerHTML = "";

    data.forEach(req => {

        let statusText = "";

        if (req.status === 0) statusText = "🟡 Pending";
        if (req.status === 1) statusText = "🟢 Accepted";
        if (req.status === 2) statusText = "✅ Completed";

        const li = document.createElement("li");

        li.innerHTML = `
        <strong>${req.title}</strong><br/>
        <small>${req.description}</small><br/>
        <span>${statusText}</span><br/>

        <button onclick="acceptRequest('${req.id}', ${req.status}, this)">Accept</button>
        <button onclick="completeRequest('${req.id}', ${req.status}, this)">Complete</button>
    `;

        list.appendChild(li);
    });
}

async function acceptRequest(id, status, btn) {

    if (status !== 0) {
        btn.style.opacity = "0.5";

        alert("❌ Only Pending requests can be accepted");
        return;
    }

    const token = localStorage.getItem("token");

    const response = await fetch(`https://localhost:44343/api/Requests/${id}/accept`, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + token
        }
    });

    if (!response.ok) {
        const error = await response.text();
        alert(error);
        return;
    }

    alert("Accepted ✅");

    // 🔥 تحديث UI بدون reload
    const parent = btn.parentElement;
    parent.querySelector("span").innerText = "🟢 Accepted";

    // تحديث status داخلي
    btn.setAttribute("onclick", `acceptRequest('${id}', 1, this)`);
    parent.querySelectorAll("button")[1]
        .setAttribute("onclick", `completeRequest('${id}', 1, this)`);
}

async function completeRequest(id, status, btn) {

    if (status !== 1) {
        btn.style.opacity = "0.5";

        alert("❌ Only Accepted requests can be completed");
        return;
    }

    const token = localStorage.getItem("token");

    const response = await fetch(`https://localhost:44343/api/Requests/${id}/complete`, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + token
        }
    });

    if (!response.ok) {
        const error = await response.text();
        alert(error);
        return;
    }

    alert("Completed ✅");

    // 🔥 تحديث UI
    const parent = btn.parentElement;
    parent.querySelector("span").innerText = "✅ Completed";

    // تحديث status
    parent.querySelectorAll("button")[0]
        .setAttribute("onclick", `acceptRequest('${id}', 2, this)`);

    btn.setAttribute("onclick", `completeRequest('${id}', 2, this)`);
}

async function assignRole() {
    const token = localStorage.getItem("token");

    const email = document.getElementById("userEmail").value;
    const role = document.getElementById("role").value;

    const response = await fetch("https://localhost:44343/api/Admin/assign-role", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify({
            email: email,
            role: role
        })
    });

    if (!response.ok) {
        const error = await response.text();
        alert(error);
        return;
    }

    alert("✅ Role assigned successfully");
}

function goToCustomer() {
    window.location.href = "/Customer/Index";
}

function goToProvider() {
    window.location.href = "/Provider/Index";
}

function parseJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]));
    } catch (e) {
        return null;
    }
}

function loadNavbar() {

    const nav = document.getElementById("navLinks");
    if (!nav) return;

    const token = localStorage.getItem("token");

    if (!token) {
        nav.innerHTML = `
            <li class="nav-item">
                <a class="nav-link" href="/Auth/Login">Login</a>
            </li>
        `;
        return;
    }

    const decoded = parseJwt(token);

    const roles = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    let role = "";

    if (Array.isArray(roles)) {
        role = roles[0];
    } else {
        role = roles;
    }

    // 🔥 بناء القائمة
    let html = `
        
    `;

    if (role === "Admin") {
        html += `
            <li class="nav-item">
                <a class="nav-link" href="/Admin/Index">Dashboard</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/Customer/Index">Customer</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/Provider/Index">Provider</a>
            </li>
        `;
    }

    if (role === "Customer") {
        html += `
            <li class="nav-item">
                <a class="nav-link" href="/Customer/Index">My Requests</a>
            </li>
        `;
    }

    if (role === "Provider") {
        html += `
            <li class="nav-item">
                <a class="nav-link" href="/Provider/Index">Requests</a>
            </li>
        `;
    }

    html += `
        <li class="nav-item">
            <a class="nav-link text-danger" href="#" onclick="logout()">Logout</a>
        </li>
    `;

    nav.innerHTML = html;
}

function logout() {
    localStorage.removeItem("token");
    window.location.href = "/Auth/Login/Index";
}

function togglePassword() {
    const passwordInput = document.getElementById("password");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    } else {
        passwordInput.type = "password";
    }
}