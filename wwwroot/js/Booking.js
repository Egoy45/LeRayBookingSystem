// ===== Booking.js (Handles Multi-Step Flow and API Submission) =====
document.addEventListener("DOMContentLoaded", () => {
    // --- Data Fields (Hidden Inputs - Must match C# DTO properties) ---
    const serviceIdInput = document.getElementById('booking-service-id');
    const totalPriceInput = document.getElementById('booking-total-price');
    const notesInput = document.getElementById('booking-notes');
    const dateStringInput = document.getElementById('booking-date-string');
    const timeSlotStringInput = document.getElementById('booking-time-slot-string');
    
    // --- Setup Elements and Variables ---
    const steps = document.querySelectorAll(".step");
    const stepIndicators = document.querySelectorAll(".steps li");
    const btns = {
        continue1: document.getElementById("continue1"),
        continue2: document.getElementById("continue2"),
        continue3: document.getElementById("continue3"),
        back2: document.getElementById("back2"),
        back3: document.getElementById("back3"),
        back4: document.getElementById("back4"),
        finish: document.getElementById("finish"),
    };
    
    let currentStep = 0;
    let selectedServices = [];
    let totalPrice = 0;
    let selectedDate = "";
    let selectedTime = "";
    
    // --- Core Functions ---
    
    function showStep(index) {
        steps.forEach((step, i) => step.classList.toggle("active", i === index));
        stepIndicators.forEach((li, i) => {
            li.classList.toggle("active", i === index);
            if (i < index) {
                li.classList.add("completed");
            } else {
                li.classList.remove("completed");
            }
        });
        currentStep = index;
    }
    
    function updateSelections() {
        // Collect all checked service boxes across all categories
        const checkedBoxes = document.querySelectorAll('input[name="services[]"]:checked');
        selectedServices = [];
        totalPrice = 0;
        
        checkedBoxes.forEach((box) => {
            const price = parseFloat(box.dataset.price || 0);
            const name = box.value;
            totalPrice += price;
            selectedServices.push({ name, price });
        });
    }
    
    function updateBundleDisplay() {
        const bundleList = document.getElementById("bundle-list");
        bundleList.innerHTML = "";
        
        if (selectedServices.length === 0) {
            bundleList.innerHTML = '<li class="empty-message" style="padding: 15px; text-align: center; color: #888;">No services selected</li>';
            return;
        }
        
        selectedServices.forEach((service, index) => {
            const li = document.createElement("li");
            li.className = "bundle-item";
            li.innerHTML = `
                <div class="bundle-number">${index + 1}</div>
                <span>${service.name}</span>
            `;
            bundleList.appendChild(li);
        });
    }
    
    function updateConfirmationUI() {
        const serviceNames = selectedServices.map((s) => `${s.name} (₱${s.price})`).join(", ");
        const totalDown = (totalPrice * 0.2).toFixed(2);
        
        // Step 3 Confirmation Details
        document.getElementById("confirm-service").textContent = serviceNames;
        document.getElementById("confirm-date").textContent = selectedDate;
        document.getElementById("confirm-time").textContent = selectedTime;
        document.getElementById("confirm-price").textContent = `₱${totalPrice.toFixed(2)}`;
        document.getElementById("confirm-downpayment").textContent = `₱${totalDown}`;
        
        // Step 4 Payment Details
        document.getElementById("payment-service").textContent = serviceNames;
        document.getElementById("payment-date").textContent = selectedDate;
        document.getElementById("payment-time").textContent = selectedTime;
        document.getElementById("payment-final-price").textContent = `₱${totalPrice.toFixed(2)}`;
        document.getElementById("payment-final-downpayment").textContent = `₱${totalDown}`;
    }
    
    // --- Service Selection Logic ---
    
    // Category Dropdown Filter (Select element above the grid)
    const serviceSelect = document.getElementById("service-select");
    const allCategories = document.querySelectorAll(".service-category"); // Targets the large service divs
    
    if (serviceSelect) {
        serviceSelect.addEventListener("change", (e) => {
            const selectedValue = e.target.value;
            
            allCategories.forEach(category => {
                if (!selectedValue || category.dataset.category === selectedValue) {
                    category.style.display = "block"; // Show category
                } else {
                    category.style.display = "none"; // Hide category
                }
            });
        });
    }
    
    // Category Checkboxes - Toggle Service Lists (in the sidebar/list)
    document.querySelectorAll(".category-checkbox-item input[type='checkbox']").forEach(checkbox => {
        checkbox.addEventListener("change", (e) => {
            const categoryId = e.target.dataset.category;
            // Target the *main* service category container using its new ID
            const serviceCategory = document.getElementById(`category-${categoryId}`);
            
            if (serviceCategory) {
                if (e.target.checked) {
                    // Show the entire service category block
                    serviceCategory.classList.add("active");
                } else {
                    // Hide the entire service category block
                    serviceCategory.classList.remove("active");
                    
                    // Uncheck all services in this category when the category checkbox is unchecked
                    serviceCategory.querySelectorAll('input[type="checkbox"][name="services[]"]').forEach(box => {
                        box.checked = false;
                    });
                }
            }
            
            updateSelections();
            updateBundleDisplay();
        });
    });
    
    // Service Checkboxes (inside the actual service lists)
    document.querySelectorAll('input[name="services[]"]').forEach((box) => {
        box.addEventListener("change", () => {
            updateSelections();
            updateBundleDisplay();
        });
    });
    
    // --- Step Validation and Navigation ---
    
    // ---- Step 1 → 2 ----
    if (btns.continue1) {
        btns.continue1.addEventListener("click", () => {
            updateSelections();
            if (selectedServices.length === 0) {
                alert("⚠️ Please select at least one service.");
                return;
            }
            // Populate hidden fields
            serviceIdInput.value = 'SERVICE_AGGREGATE';
            totalPriceInput.value = totalPrice.toFixed(2);
            notesInput.value = `Selected Services: ${selectedServices.map(s => s.name).join('; ')}`; 
            
            showStep(1);
        });
    }
    
    // ---- Step 2 → 3 (Confirmation) ----
    if (btns.continue2) {
        btns.continue2.addEventListener("click", () => {
            const selectedSlotBtn = document.querySelector(".timeslots button.selected");
            if (!selectedDate || !selectedSlotBtn) {
                alert("⚠️ Please select a date and time first.");
                return;
            }
            
            selectedTime = selectedSlotBtn.textContent.trim();
            
            // Populate hidden fields for Date/Time
            dateStringInput.value = selectedDate;
            timeSlotStringInput.value = selectedTime;
            
            updateConfirmationUI();
            showStep(2); 
        });
    }
    
    // ---- Step 3 → 4 (Payment) ----
    if (btns.continue3) {
        btns.continue3.addEventListener("click", () => {
            showStep(3);
        });
    }
    
    // ---- Final Submission (Step 4) ----
    if (btns.finish) {
        btns.finish.addEventListener("click", async (e) => {
            e.preventDefault();
            
            const receiptInput = document.getElementById("receipt");
            if (!receiptInput.files.length) {
                alert("⚠️ Please upload a payment receipt file.");
                return;
            }
            
            const form = document.querySelector("form");
            const formData = new FormData(form); 
            
            const token = localStorage.getItem('authToken');
            if (!token) {
                alert("⚠️ You must be logged in to complete the booking.");
                window.location.href = '/';
                return;
            }
            
            try {
                const response = await fetch("/api/Bookings", {
                    method: "POST",
                    body: formData,
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });
                
                if (!response.ok) {
                    const error = await response.json();
                    const errorMessage = error.Message || error.title || (error.errors ? JSON.stringify(error.errors) : response.statusText);
                    console.error("Booking error:", error);
                    alert("❌ Failed to create booking: " + errorMessage);
                    return;
                }
                
                const result = await response.json();
                console.log("Booking success:", result);
                
                alert("✅ Booking successful! Appointment ID: " + result.id);
                window.location.href = "/Home/Index";
            } catch (err) {
                console.error("Network error:", err);
                alert("⚠️ Network error while submitting booking.");
            }
        });
    }
    
    // ---- Time Slot Selection ----
    document.querySelectorAll(".timeslots button").forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            document.querySelectorAll(".timeslots button").forEach(b => b.classList.remove("selected"));
            btn.classList.add("selected");
        });
    });
    
    // ---- Back Buttons ----
    if (btns.back2) btns.back2.addEventListener("click", () => showStep(0));
    if (btns.back3) btns.back3.addEventListener("click", () => showStep(1));
    if (btns.back4) btns.back4.addEventListener("click", () => showStep(2));
    
    // Initial load
    showStep(0);
    updateBundleDisplay();
    
    // Initialize date picker logic
    if (typeof flatpickr !== "undefined") {
        flatpickr("#date-picker", {
            inline: true,
            dateFormat: "m/d/Y",
            minDate: "today",
            onChange: (selectedDates, dateStr) => {
                selectedDate = dateStr;
            },
        });
    }
});