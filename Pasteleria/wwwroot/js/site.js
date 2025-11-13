// Menu móvil
document.addEventListener('DOMContentLoaded', function () {
    const hamburger = document.querySelector('.hamburger');
    const navMenu = document.querySelector('.nav-menu');

    if (hamburger) {
        hamburger.addEventListener('click', function () {
            navMenu.classList.toggle('active');

            // Animación del hamburger
            const spans = hamburger.querySelectorAll('span');
            spans[0].style.transform = navMenu.classList.contains('active')
                ? 'rotate(45deg) translate(5px, 5px)'
                : 'none';
            spans[1].style.opacity = navMenu.classList.contains('active') ? '0' : '1';
            spans[2].style.transform = navMenu.classList.contains('active')
                ? 'rotate(-45deg) translate(7px, -6px)'
                : 'none';
        });
    }

    // Cerrar menu al hacer click en un link
    const navLinks = document.querySelectorAll('.nav-menu a');
    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            if (navMenu) {
                navMenu.classList.remove('active');
                if (hamburger) {
                    const spans = hamburger.querySelectorAll('span');
                    spans[0].style.transform = 'none';
                    spans[1].style.opacity = '1';
                    spans[2].style.transform = 'none';
                }
            }
        });
    });

    // Smooth scroll para los enlaces de anclaje
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Animación de entrada para las cards
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Observar elementos que se deben animar
    const animatedElements = document.querySelectorAll(
        '.product-card, .feature-card, .category-card, .testimonial-card'
    );

    animatedElements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(el);
    });

    // Simulación de agregar al carrito (placeholder)
    // SOLO aplicar esto en páginas que tengan el grid de productos
    // EXCLUIR botones que sean enlaces (tienen href) o tengan clase especial
    const productsGrid = document.querySelector('.products-grid');
    if (productsGrid) {
        const addToCartButtons = productsGrid.querySelectorAll('.product-card .btn-primary:not(.btn-ver-categoria)');
        const cartCount = document.querySelector('.cart-count');
        let count = 0;

        addToCartButtons.forEach(button => {
            // Verificar que NO sea un enlace y NO sea un botón de tipo submit
            if (button.tagName !== 'A' && button.type !== 'submit') {
                button.addEventListener('click', function (e) {
                    e.preventDefault();
                    count++;
                    if (cartCount) {
                        cartCount.textContent = count;
                    }

                    // Animación del botón
                    this.textContent = '¡Agregado!';
                    this.style.background = '#27ae60';

                    setTimeout(() => {
                        this.textContent = 'Agregar';
                        this.style.background = '';
                    }, 1500);

                    // Animación del contador del carrito
                    if (cartCount) {
                        cartCount.style.transform = 'scale(1.3)';
                        setTimeout(() => {
                            cartCount.style.transform = 'scale(1)';
                        }, 300);
                    }
                });
            }
        });
    }

    // Efecto parallax suave en el hero - SOLO en la página principal
    const hero = document.querySelector('.hero');
    if (hero && !document.querySelector('form')) { // No aplicar parallax en páginas con formularios
        window.addEventListener('scroll', function () {
            const scrolled = window.pageYOffset;
            hero.style.transform = `translateY(${scrolled * 0.5}px)`;
        });
    }
});