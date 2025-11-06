document.addEventListener('DOMContentLoaded', () => {
  const hamburger = document.getElementById('hamburger');
  const navMenu = document.getElementById('nav-menu');
  const accountBtn = document.getElementById('accountBtn');
  const sidebar = document.getElementById('sidebar');
  const closeSidebar = document.getElementById('closeSidebar');
  const bookNowBtn = document.getElementById('bookNowBtn');

  // Hamburger menu toggle
  if (hamburger && navMenu) {
    hamburger.addEventListener('click', () => {
      navMenu.classList.toggle('active');
    });
  }

  // Sidebar toggle
  if (accountBtn && sidebar) {
    accountBtn.addEventListener('click', () => {
      sidebar.classList.toggle('active');
    });
  }
  if (closeSidebar && sidebar) {
    closeSidebar.addEventListener('click', () => {
      sidebar.classList.remove('active');
    });
  }

  // --- FIX: Changed 'Booking.html' to the correct ASP.NET route '/Home/Booking' ---
  if (bookNowBtn) {
    bookNowBtn.addEventListener('click', () => {
      window.location.href = "/Home/Booking"; 
    });
  }

  // Carousel functionality
  const slides = document.querySelectorAll('.carousel-slide');
  let currentSlide = 0;
  if (slides.length > 0) {
    const showSlide = (n) => {
      slides.forEach((slide) => slide.classList.remove('active'));
      slides[n].classList.add('active');
    };

    const nextSlide = () => {
      currentSlide = (currentSlide + 1) % slides.length;
      showSlide(currentSlide);
    };

    // Auto-play
    let slideInterval = setInterval(nextSlide, 3000);

    // Optional: Pause on hover
    const carousel = document.querySelector('.hero-carousel');
    if (carousel) {
      carousel.addEventListener('mouseenter', () => clearInterval(slideInterval));
      carousel.addEventListener('mouseleave', () => {
        slideInterval = setInterval(nextSlide, 3000);
      });
    }
  }

  // Testimonial slider
  const testimonials = [
    {
      name: 'Client’s name',
      quote:
        '"I have always had a wonderful experience. The receptionists at Le’Ray are warm and welcoming, and the professionals are incredibly knowledgeable when it comes to skincare and wellness. They offer helpful advice tailored to my needs. I’ve recommended Le’Ray to friends, and they’ve all had the same amazing experience. Great job, Le’Ray Aesthetic and Wellness Center!"',
      rating: 5,
    },
    // Add more testimonials here if needed
  ];

  let currentTestimonial = 0;
  const quoteEl = document.querySelector('.testimonial-box .quote');
  const nameEl = document.querySelector('.testimonial-box h3');
  const starsEl = document.querySelector('.testimonial-box .stars');
  const dots = document.querySelectorAll('.testimonial .dot');

  const showTestimonial = (n) => {
    if (quoteEl && nameEl && starsEl) {
      const testimonial = testimonials[n];
      quoteEl.textContent = testimonial.quote;
      nameEl.textContent = testimonial.name;
      starsEl.textContent = '★'.repeat(testimonial.rating);
      dots.forEach((dot, index) => {
        dot.classList.toggle('active', index === n);
      });
    }
  };

  if (document.getElementById('prevBtn')) {
    document.getElementById('prevBtn').addEventListener('click', () => {
      currentTestimonial = (currentTestimonial - 1 + testimonials.length) % testimonials.length;
      showTestimonial(currentTestimonial);
    });
  }

  if (document.getElementById('nextBtn')) {
    document.getElementById('nextBtn').addEventListener('click', () => {
      currentTestimonial = (currentTestimonial + 1) % testimonials.length;
      showTestimonial(currentTestimonial);
    });
  }
});

// Password toggle for signup/login modals
function togglePassword(fieldId, icon) {
  const field = document.getElementById(fieldId);
  if (field) {
    if (field.type === 'password') {
      field.type = 'text';
      icon.innerHTML = '<i class="fa-solid fa-eye-slash"></i>';
    } else {
      field.type = 'password';
      icon.innerHTML = '<i class="fa-solid fa-eye"></i>';
    }
  }
}
const sidebar = document.getElementById("sidebar");
  const sidebarOverlay = document.getElementById("sidebarOverlay");
  const closeSidebar = document.getElementById("closeSidebar");
  const openSidebarBtn = document.getElementById("openSidebar"); // your account icon button

  if (openSidebarBtn && sidebar && sidebarOverlay && closeSidebar) {
    openSidebarBtn.addEventListener("click", () => {
      sidebar.classList.add("active");
      sidebarOverlay.classList.add("active");
    });

    closeSidebar.addEventListener("click", () => {
      sidebar.classList.remove("active");
      sidebarOverlay.classList.remove("active");
    });

    sidebarOverlay.addEventListener("click", () => {
      sidebar.classList.remove("active");
      sidebarOverlay.classList.remove("active");
    });
  }
