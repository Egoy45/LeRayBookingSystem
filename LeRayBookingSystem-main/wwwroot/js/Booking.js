document.addEventListener("DOMContentLoaded", () => {
  // ===== Elements =====
  const steps = document.querySelectorAll(".step");
  const stepIndicators = document.querySelectorAll(".steps li");
  let currentStep = 0;

  function showStep(index) {
    steps.forEach((step, i) => {
      step.classList.toggle("active", i === index);
      if (stepIndicators[i])
        stepIndicators[i].classList.toggle("active", i === index);
    });
    currentStep = index;
  }

  // ===== Accordion Category Behavior =====
  document.querySelectorAll(".category").forEach((category) => {
    const header = category.querySelector(".category-header");
    const list = category.querySelector("ol");

    list.style.maxHeight = "0px";
    list.style.overflow = "hidden";
    list.style.transition = "max-height 0.4s ease";

    header.addEventListener("click", () => {
      const isOpen = category.classList.contains("open");

      document.querySelectorAll(".category").forEach((c) => {
        if (c !== category) {
          c.classList.remove("open");
          c.querySelector("ol").style.maxHeight = "0px";
        }
      });

      category.classList.toggle("open", !isOpen);
      list.style.maxHeight = !isOpen ? list.scrollHeight + "px" : "0px";
    });
  });

  // ===== Variables for Selections =====
  let selectedService = "";
  let selectedDate = "";
  let selectedTime = "";

  // ===== Service Dropdown (Category Filter) =====
  const serviceSelect = document.getElementById("service-select");
  const categories = document.querySelectorAll(".category");

  if (serviceSelect) {
    serviceSelect.addEventListener("change", () => {
      const selected = serviceSelect.value;
      categories.forEach((cat) => {
        cat.style.display = cat.dataset.category === selected ? "block" : "none";
      });
    });
  }

  // ===== Radio Button Service Selection =====
  document.querySelectorAll('input[name="service"]').forEach((radio) => {
    radio.addEventListener("change", (e) => {
      selectedService = e.target.value;
      selectedPrice = e.target.dataset.price;
    });
  });

  // ===== Date Picker =====
  if (typeof flatpickr !== "undefined") {
    flatpickr("#date-picker", {
      inline: true,
      dateFormat: "F j, Y",
      minDate: "today",
      onChange: (selectedDates, dateStr) => {
        selectedDate = dateStr;
      },
    });
  }

  // ===== Time Slot Selection =====
  document.querySelectorAll(".timeslots button").forEach((btn) => {
    btn.addEventListener("click", () => {
      document
        .querySelectorAll(".timeslots button")
        .forEach((b) => b.classList.remove("selected"));
      btn.classList.add("selected");
      selectedTime = btn.textContent.trim();
    });
  });

  // ===== Step Buttons =====
  const btns = {
    continue1: document.getElementById("continue1"),
    continue2: document.getElementById("continue2"),
    continue3: document.getElementById("continue3"),
    back2: document.getElementById("back2"),
    back3: document.getElementById("back3"),
    back4: document.getElementById("back4"),
    finish: document.getElementById("finish"),
  };

  // ===== Step 1 → 2 =====
  if (btns.continue1) {
    btns.continue1.addEventListener("click", () => {
      if (!selectedService) {
        alert("⚠️ Please select a service first.");
        return;
      }
      showStep(1);
    });
  }

  // ===== Step 2 → 3 =====
  if (btns.continue2) {
    btns.continue2.addEventListener("click", () => {
      if (!selectedDate || !selectedTime) {
        alert("⚠️ Please select a date and time first.");
        return;
      }
      document.getElementById("confirm-service").textContent = selectedService;
      document.getElementById("confirm-date").textContent = selectedDate;
      document.getElementById("confirm-time").textContent = selectedTime;
      showStep(2);
    });
  }

  // ===== Step 3 → 4 =====
  if (btns.continue3) {
    btns.continue3.addEventListener("click", () => {
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
      alert(
        `🎉 Booking complete!\n\nService: ${selectedService}\nDate: ${selectedDate}\nTime: ${selectedTime}`
      );
      window.location.href = "../pages/Index.html";
    });
  }

  // ===== Receipt Upload Validation =====
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
