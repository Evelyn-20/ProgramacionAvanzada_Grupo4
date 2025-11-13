// catalogo.js - JavaScript para la gestión del catálogo de productos

// Variables globales
var maxCantidad = 1;

document.addEventListener('DOMContentLoaded', function () {
    inicializarCatalogo();
});

// Inicializar funcionalidades del catálogo
function inicializarCatalogo() {
    const paginaActual = obtenerPaginaActual();

    switch (paginaActual) {
        case 'index':
            inicializarPaginaCategorias();
            break;
        case 'detalle':
            inicializarPaginaDetalle();
            break;
        case 'productos':
            inicializarPaginaProductos();
            break;
    }
}

// Detectar página actual basándose en elementos del DOM
function obtenerPaginaActual() {
    if (document.querySelector('.categoria-card')) {
        return 'index';
    } else if (document.getElementById('cantidad')) {
        return 'detalle';
    } else if (document.querySelector('.product-card')) {
        return 'productos';
    }
    return 'unknown';
}

// ========================================
// PÁGINA DE CATEGORÍAS (Index.cshtml)
// ========================================
function inicializarPaginaCategorias() {
    console.log('Página de categorías cargada');

    // Desactivar función agregarAlCarrito si existe globalmente
    if (typeof agregarAlCarrito !== 'undefined') {
        console.log('Función agregarAlCarrito deshabilitada en página de categorías');
    }

    // Asegurar que los botones de categoría funcionen correctamente
    const botonesCategoria = document.querySelectorAll('.btn-ver-categoria');
    botonesCategoria.forEach(function (boton) {
        boton.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            console.log('Navegando a:', href);
        });
    });

    // Click en las tarjetas de categoría
    const productCards = document.querySelectorAll('.categoria-card');
    productCards.forEach(function (card) {
        card.style.cursor = 'pointer';
        card.addEventListener('click', function (e) {
            // Si el click es en el enlace, dejarlo pasar
            if (e.target.closest('.btn-ver-categoria')) {
                return true;
            }
            // Si el click es en la tarjeta, redirigir
            const link = this.querySelector('.btn-ver-categoria');
            if (link) {
                window.location.href = link.getAttribute('href');
            }
        });

        // Efecto hover
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-10px)';
        });
        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
        });
    });
}

// ========================================
// PÁGINA DE PRODUCTOS (Productos.cshtml)
// ========================================
function inicializarPaginaProductos() {
    console.log('Página de productos cargada');

    // Efectos hover en las tarjetas de productos
    const productCards = document.querySelectorAll('.product-card');
    productCards.forEach(function (card) {
        card.addEventListener('mouseenter', function () {
            const img = this.querySelector('.product-image img');
            if (img) {
                img.style.transform = 'scale(1.05)';
                img.style.transition = 'transform 0.3s ease';
            }
        });
        card.addEventListener('mouseleave', function () {
            const img = this.querySelector('.product-image img');
            if (img) {
                img.style.transform = 'scale(1)';
            }
        });
    });
}

// ========================================
// PÁGINA DE DETALLE (DetalleProducto.cshtml)
// ========================================
function inicializarPaginaDetalle() {
    console.log('Página de detalle de producto cargada');

    // Obtener cantidad máxima del input
    const inputCantidad = document.getElementById('cantidad');
    if (inputCantidad) {
        maxCantidad = parseInt(inputCantidad.getAttribute('max')) || 1;
    }

    // Configurar botones de cantidad
    configurarBotonesCantidad();
}

// Configurar efectos de los botones de cantidad
function configurarBotonesCantidad() {
    const botonesCantidad = document.querySelectorAll('button[onclick*="cambiarCantidad"]');
    botonesCantidad.forEach(function (btn) {
        btn.addEventListener('mouseenter', function () {
            this.style.transform = 'scale(1.1)';
            this.style.transition = 'transform 0.2s ease';
        });
        btn.addEventListener('mouseleave', function () {
            this.style.transform = 'scale(1)';
        });
    });
}

// Cambiar cantidad de producto
function cambiarCantidad(cambio) {
    const input = document.getElementById('cantidad');
    if (!input) return;

    let cantidad = parseInt(input.value) + cambio;

    if (cantidad < 1) cantidad = 1;
    if (cantidad > maxCantidad) cantidad = maxCantidad;

    input.value = cantidad;

    // Animación del input
    input.style.transform = 'scale(1.1)';
    setTimeout(function () {
        input.style.transform = 'scale(1)';
    }, 200);
}

// Agregar producto al carrito
function agregarAlCarrito() {
    const inputCantidad = document.getElementById('cantidad');
    if (!inputCantidad) return;

    const cantidad = parseInt(inputCantidad.value);
    const nombreProducto = document.querySelector('h2')?.textContent || 'Producto';

    // Verificar si SweetAlert2 está disponible
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'success',
            title: '¡Producto agregado!',
            html: `Se agregaron <strong>${cantidad}</strong> unidad${cantidad > 1 ? 'es' : ''} de <strong>${nombreProducto}</strong> al carrito`,
            showConfirmButton: true,
            confirmButtonText: 'Continuar comprando',
            showCancelButton: true,
            cancelButtonText: 'Ver carrito',
            confirmButtonColor: 'var(--primary-color)',
            cancelButtonColor: 'var(--secondary-color)'
        }).then(function (result) {
            if (!result.isConfirmed) {
                // Redirigir al carrito
                window.location.href = '/Carrito/Carrito';
            }
        });
    } else {
        // Fallback si no está SweetAlert2
        alert(`Se agregaron ${cantidad} unidad${cantidad > 1 ? 'es' : ''} de ${nombreProducto} al carrito`);

        // Preguntar si quiere ir al carrito
        if (confirm('¿Desea ver su carrito?')) {
            window.location.href = '/Carrito/Carrito';
        }
    }

    // Actualizar contador del carrito en el navbar
    actualizarContadorCarrito(cantidad);
}

// Actualizar contador del carrito en el navbar
function actualizarContadorCarrito(cantidadAgregada) {
    const cartCount = document.querySelector('.cart-count');
    if (cartCount) {
        let count = parseInt(cartCount.textContent) || 0;
        count += cantidadAgregada;
        cartCount.textContent = count;

        // Animación del contador
        cartCount.style.transform = 'scale(1.3)';
        cartCount.style.transition = 'transform 0.3s ease';
        setTimeout(function () {
            cartCount.style.transform = 'scale(1)';
        }, 300);
    }
}

// Función auxiliar para formatear precios
function formatearPrecio(precio) {
    return '₡' + precio.toLocaleString('es-CR', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    });
}

// Función para lazy loading de imágenes (opcional, para mejorar rendimiento)
function inicializarLazyLoading() {
    const imagenes = document.querySelectorAll('img[data-src]');

    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.removeAttribute('data-src');
                    imageObserver.unobserve(img);
                }
            });
        });

        imagenes.forEach(function (img) {
            imageObserver.observe(img);
        });
    } else {
        // Fallback para navegadores sin soporte
        imagenes.forEach(function (img) {
            img.src = img.dataset.src;
        });
    }
}