document.addEventListener("DOMContentLoaded", () => {
  // ===== Book Now Button =====
  const bookBtn = document.getElementById("bookBtn");
  if (bookBtn) {
    bookBtn.addEventListener("click", () => {
      window.location.href = "../Booking/Booking.html";
    });
  }

  // ===== Steps Logic =====
  const steps = document.querySelectorAll(".step");
  const stepIndicators = document.querySelectorAll(".steps li");
  let currentStep = 0;

  function showStep(index) {
    steps.forEach((s, i) => s.classList.toggle("active", i === index));
    stepIndicators.forEach((li, i) => li.classList.toggle("active", i === index));
    currentStep = index;
  }

  // ===== Buttons =====
  const btns = {
    continue1: document.getElementById("continue1"),
    continue2: document.getElementById("continue2"),
    back2: document.getElementById("back2"),
    back3: document.getElementById("back3"),
    confirmBtn: document.getElementById("confirmBtn"),
    finish: document.querySelector(".finish")
  };

  // ===== Variables to Store Selections =====
  let selectedService = "";
  let selectedDate = "";
  let selectedTime = "";

  // ===== Service Selection =====
  const serviceSelect = document.getElementById("service-select");
  if (serviceSelect) {
    serviceSelect.addEventListener("change", () => {
      selectedService = serviceSelect.options[serviceSelect.selectedIndex].text;
    });
  }

  // ===== Date Picker =====
  if (typeof flatpickr !== "undefined") {
    flatpickr("#date-picker", {
      inline: true,
      dateFormat: "F j, Y",
      minDate: "today",
      onChange: function (selectedDates, dateStr) {
        selectedDate = dateStr;
      }
    });
  }

  // ===== Time Slot Selection =====
  document.querySelectorAll(".timeslots button").forEach(btn => {
    btn.addEventListener("click", () => {
      document.querySelectorAll(".timeslots button").forEach(b => b.classList.remove("selected"));
      btn.classList.add("selected");
      selectedTime = btn.textContent.trim();
    });
  });

  // ===== Navigation =====
  if (btns.continue1) {
    btns.continue1.addEventListener("click", () => showStep(1));
  }

  if (btns.continue2) {
    btns.continue2.addEventListener("click", () => {
      if (!selectedService || !selectedDate || !selectedTime) {
        alert("⚠️ Please select a service, date, and time before continuing.");
        return;
      }

      // Update confirmation text
      document.getElementById("confirm-service").textContent = selectedService;
      document.getElementById("confirm-date").textContent = selectedDate;
      document.getElementById("confirm-time").textContent = selectedTime;

      showStep(2); // Move to confirmation step
    });
  }

  if (btns.back2) {
    btns.back2.addEventListener("click", () => showStep(0));
  }

  if (btns.back3) {
    btns.back3.addEventListener("click", () => showStep(1));
  }

  // ===== Confirm Button =====
  if (btns.confirmBtn) {
    btns.confirmBtn.addEventListener("click", () => {
      alert("✅ Booking Confirmed!\nThank you for choosing LeRay Aesthetic and Wellness Center.");
      showStep(3);
    });
  }

  // ===== Finish Button =====
  if (btns.finish) {
    btns.finish.addEventListener("click", () => {
      alert("🎉 Booking complete! Thank you for choosing LeRay.");
    });
  }
});
