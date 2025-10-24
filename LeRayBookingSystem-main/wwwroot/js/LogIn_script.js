// ===== login.js (Handles Login & Signup Modals) =====
document.addEventListener("DOMContentLoaded", function () {
  const bookNowBtn = document.getElementById("bookNowBtn");
  const signupModal = document.getElementById("signupModal");
  const loginModal = document.getElementById("loginModal");

  const openLoginLink = document.getElementById("openLogin");
  const openSignupLink = document.getElementById("openSignup");
  const backToSignup = document.getElementById("backToSignup");

  const closeSignup = signupModal ? signupModal.querySelector(".close") : null;
  const closeLogin = loginModal ? loginModal.querySelector(".close-login") : null;

  // ===== Open Sign Up modal =====
  if (bookNowBtn && signupModal) {
    bookNowBtn.addEventListener("click", (e) => {
      e.preventDefault();
      signupModal.style.display = "flex";
    });
  }

  // ===== Switch to Login modal =====
  if (openLoginLink && signupModal && loginModal) {
    openLoginLink.addEventListener("click", (e) => {
      e.preventDefault();
      signupModal.style.display = "none";
      loginModal.style.display = "flex";
    });
  }

  // ===== Switch back to Sign Up modal =====
  if (openSignupLink && signupModal && loginModal) {
    openSignupLink.addEventListener("click", (e) => {
      e.preventDefault();
      loginModal.style.display = "none";
      signupModal.style.display = "flex";
    });
  }

  // ===== Back arrow to Sign Up =====
  if (backToSignup && signupModal && loginModal) {
    backToSignup.addEventListener("click", () => {
      loginModal.style.display = "none";
      signupModal.style.display = "flex";
    });
  }

  // ===== Close modals =====
  if (closeSignup) {
    closeSignup.addEventListener("click", () => {
      signupModal.style.display = "none";
    });
  }

  if (closeLogin) {
    closeLogin.addEventListener("click", () => {
      loginModal.style.display = "none";
    });
  }

  // ===== Close when clicking outside =====
  window.addEventListener("click", (event) => {
    if (event.target === signupModal) signupModal.style.display = "none";
    if (event.target === loginModal) loginModal.style.display = "none";
  });
});
