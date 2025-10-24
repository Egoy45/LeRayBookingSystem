document.addEventListener("DOMContentLoaded", () => {
  // ===== Elements =====
  const steps = document.querySelectorAll(".step");
  const stepIndicators = document.querySelectorAll(".steps li");
  let currentStep = 0;

  // ===== Helper Function: Show Step =====
  function showStep(index) {
    steps.forEach((step, i) => {
      step.classList.toggle("active", i === index);
      stepIndicators[i].classList.toggle("active", i === index);
    });
    currentStep = index;
  }

  // ===== Step Navigation Buttons =====
  const btns = {
    continue1: document.getElementById("continue1"),
    continue2: document.getElementById("continue2"),
    continue3: document.getElementById("continue3"),
    back2: document.getElementById("back2"),
    back3: document.getElementById("back3"),
    back4: document.getElementById("back4"),
    finish: document.getElementById("finish")
  };

  // ===== Booking Data =====
  let selectedService = "";
  let selectedDate = "";
  let selectedTime = "";

  // ===== Step 1: Service Selection =====
  const serviceSelect = document.getElementById("service-select");
  if (serviceSelect) {
    serviceSelect.addEventListener("change", () => {
      selectedService = serviceSelect.options[serviceSelect.selectedIndex].text;
    });
  }

  // ===== Step 2: Date Picker =====
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

  // ===== Step 2: Time Slot Selection =====
  document.querySelectorAll(".timeslots button").forEach(btn => {
    btn.addEventListener("click", () => {
      document.querySelectorAll(".timeslots button").forEach(b => b.classList.remove("selected"));
      btn.classList.add("selected");
      selectedTime = btn.textContent.trim();
    });
  });

  // ===== Navigation Logic =====
  // Step 1 → Step 2
  if (btns.continue1) {
    btns.continue1.addEventListener("click", () => {
      if (!selectedService) {
        return;
      }
      showStep(1);
    });
  }

  // Step 2 → Step 3
  if (btns.continue2) {
    btns.continue2.addEventListener("click", () => {
      if (!selectedDate || !selectedTime) {
        return;
      }

      // Update confirmation details
      document.getElementById("confirm-service").textContent = selectedService;
      document.getElementById("confirm-date").textContent = selectedDate;
      document.getElementById("confirm-time").textContent = selectedTime;

      showStep(2);
    });
  }

  // Step 3 → Step 4 (Payment)
  if (btns.continue3) {
    btns.continue3.addEventListener("click", () => {
      // Copy confirmation details to payment step
      document.getElementById("payment-service").textContent = selectedService;
      document.getElementById("payment-date").textContent = selectedDate;
      document.getElementById("payment-time").textContent = selectedTime;

      showStep(3);
    });
  }

  // ===== Back Buttons =====
  if (btns.back2) btns.back2.addEventListener("click", () => showStep(0));
  if (btns.back3) btns.back3.addEventListener("click", () => showStep(1));
  if (btns.back4) btns.back4.addEventListener("click", () => showStep(2));

  // ===== Finish Button =====
  if (btns.finish) {
    btns.finish.addEventListener("click", () => {
      alert("🎉 Booking complete! Thank you for choosing LeRay Aesthetic and Wellness Center.");
      window.location.href = "../index.html"; // Redirect to home or another page
    });
  }

  // ===== Receipt Upload (Optional Visual Feedback) =====
  const receiptInput = document.getElementById("receipt");
  if (receiptInput) {
    receiptInput.addEventListener("change", () => {
      const file = receiptInput.files[0];
      if (file) {
        const validTypes = ["image/jpeg", "image/png", "image/jpg"];
        if (!validTypes.includes(file.type)) {
          alert("⚠️ Please upload a valid image file (.jpg, .jpeg, .png).");
          receiptInput.value = "";
          return;
        }
        if (file.size > 5 * 1024 * 1024) {
          alert("⚠️ File is too large. Max size is 5MB.");
          receiptInput.value = "";
          return;
        }
        alert(`✅ Receipt uploaded: ${file.name}`);
      }
    });
  }
});
