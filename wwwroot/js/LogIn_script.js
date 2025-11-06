// ===== login.js (Handles Login & Signup Modals + API integration) =====
document.addEventListener("DOMContentLoaded", function () {
    // --- Elements ---
    const bookNowBtn = document.getElementById("bookNowBtn");
    const signupModal = document.getElementById("signupModal");
    const loginModal = document.getElementById("loginModal");

    const openLoginLink = document.getElementById("openLogin");
    const openSignupLink = document.getElementById("openSignup");

    const closeSignup = signupModal?.querySelector(".close");
    const closeLogin = loginModal?.querySelector(".close-login");

    const signupForm = document.getElementById("signupForm");
    const loginForm = document.getElementById("loginForm");

    // --- Utility Functions ---
    function openModal(modal) {
        if (modal) {
            modal.style.display = "flex";
            document.body.classList.add("modal-open");
        }
    }

    function closeModal(modal) {
        if (modal) {
            modal.style.display = "none";
            document.body.classList.remove("modal-open");
        }
    }

    // --- Critical: Get Redirect URL ---
    const urlParams = new URLSearchParams(window.location.search);
    const returnUrl = urlParams.get("ReturnUrl");

    // --- Book Now button opens SignUp modal ---
    if (bookNowBtn) {
        bookNowBtn.addEventListener("click", (e) => {
            e.preventDefault();
            openModal(signupModal);
        });
    }

    // --- Auto open login modal if redirected ---
    if (urlParams.has("ReturnUrl")) {
        openModal(loginModal);
    }

    // --- Switch between modals ---
    openLoginLink?.addEventListener("click", (e) => {
        e.preventDefault();
        closeModal(signupModal);
        openModal(loginModal);
    });

    openSignupLink?.addEventListener("click", (e) => {
        e.preventDefault();
        closeModal(loginModal);
        openModal(signupModal);
    });

    // --- Close modals ---
    closeSignup?.addEventListener("click", () => closeModal(signupModal));
    closeLogin?.addEventListener("click", () => closeModal(loginModal));

    // --- Close when clicking outside modal ---
    window.addEventListener("click", (event) => {
        if (event.target === signupModal) closeModal(signupModal);
        if (event.target === loginModal) closeModal(loginModal);
    });

    // ===========================================================
    // 🟢 SIGNUP HANDLER
    // ===========================================================
    signupForm?.addEventListener("submit", async (e) => {
        e.preventDefault();

        const firstName = document.getElementById("firstName")?.value.trim() || "";
        const lastName = document.getElementById("lastName")?.value.trim() || "";
        const email = document.getElementById("signupEmail")?.value.trim() || "";
        const password = document.getElementById("password")?.value.trim() || "";
        const confirmPassword = document.getElementById("confirmPassword")?.value.trim() || "";
        const captchaToken = "mock-captcha-token"; // placeholder

        if (password !== confirmPassword) {
            alert("❌ Passwords do not match!");
            return;
        }

        const registerDto = {
            firstName,
            lastName,
            email,
            password,
            confirmPassword,
            captchaToken
        };

        try {
            const res = await fetch("/api/AccountApi/signup", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "same-origin",
                body: JSON.stringify(registerDto)
            });

            const result = await res.json();

            if (res.ok) {
                alert("✅ Registration successful! Please log in.");
                closeModal(signupModal);
                openModal(loginModal);
            } else {
                const errorMessage =
                    result.Message ||
                    (result.errors ? JSON.stringify(result.errors) : "An unknown error occurred.");
                alert("❌ Signup failed: " + errorMessage);
            }
        } catch (error) {
            console.error("Error during signup:", error);
            alert("❌ Network error while signing up.");
        }
    });

    // ===========================================================
    // 🟢 LOGIN HANDLER
    // ===========================================================
    loginForm?.addEventListener("submit", async (e) => {
        e.preventDefault();

        const emailOrUsername = document.getElementById("login-username")?.value.trim() || "";
        const password = document.getElementById("login-password")?.value.trim() || "";

        const loginDto = { EmailOrUsername: emailOrUsername, Password: password };

        try {
            const res = await fetch("/api/AccountApi/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "same-origin", // 🔑 ensures auth cookie is saved
                body: JSON.stringify(loginDto)
            });

            const result = await res.json();

            if (res.ok) {
                alert("✅ Login successful!");

                // ❌ REMOVE TOKEN STORAGE — using cookie-based login
                // ✅ Redirect correctly
                closeModal(loginModal);

                if (returnUrl) {
                    window.location.href = decodeURIComponent(returnUrl);
                } else {
                    window.location.href = "/Home/Index"; // default landing page
                }
            } else {
                const errorMessage = result.Message || "Invalid credentials. Please try again.";
                alert("❌ Login failed: " + errorMessage);
            }
        } catch (error) {
            console.error("Error during login:", error);
            alert("❌ Network error while logging in.");
        }
    });
});
