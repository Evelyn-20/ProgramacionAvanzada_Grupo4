// Variables globales
var maxCantidad = 1;

document.addEventListener('DOMContentLoaded', function () {
    inicializarCatalogo();
    // Cargar cantidad del carrito al iniciar
    actualizarContadorCarritoDesdeServidor();
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

// Página de las Categorias
function inicializarPaginaCategorias() {

    // Asegurar que los botones de categoría funcionen correctamente
    const botonesCategoria = document.querySelectorAll('.btn-ver-categoria');
    botonesCategoria.forEach(function (boton) {
        boton.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
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

// Página de Productos
function inicializarPaginaProductos() {

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

// Página de Detalles del Producto
function inicializarPaginaDetalle() {

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
    if (cantidad > maxCantidad) {
        cantidad = maxCantidad;
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'warning',
                title: 'Stock máximo',
                text: `Solo hay ${maxCantidad} unidades disponibles`,
                confirmButtonColor: '#d4825c',
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 2000
            });
        }
    }

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
    if (!inputCantidad) {
        return;
    }

    const cantidad = parseInt(inputCantidad.value) || 1;

    // Obtener el ID del producto desde la URL
    const urlParts = window.location.pathname.split('/');
    const idProducto = parseInt(urlParts[urlParts.length - 1]);

    if (!idProducto || isNaN(idProducto)) {
        mostrarMensaje('Error al identificar el producto', 'error');
        return;
    }

    // Mostrar loading
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            title: 'Agregando al carrito...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    }

    // Realizar peticion AJAX al servidor
    
    fetch('/Carrito/AgregarAlCarrito', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `idProducto=${idProducto}&cantidad=${cantidad}`
    })
        .then(response => {
            console.log('Estado de respuesta:', response.status);

            // Si no está autenticado, el servidor puede retornar 401
            if (response.status === 401 || response.status === 403) {
                throw new Error('NO_AUTH');
            }

            if (!response.ok) {
                throw new Error('ERROR_SERVIDOR');
            }

            return response.json();
        })
        .then(data => {
            console.log('Respuesta del servidor:', data);

            if (data.success) {
                // Actualizar contador del carrito
                actualizarContadorCarrito(data.cantidadTotal);

                // Obtener nombre del producto
                const nombreProducto = document.querySelector('h2')?.textContent || 'Producto';

                // Mostrar mensaje de éxito
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'success',
                        title: '¡Producto agregado!',
                        html: `Se agregaron <strong>${cantidad}</strong> unidad${cantidad > 1 ? 'es' : ''} de <strong>${nombreProducto}</strong> al carrito`,
                        showConfirmButton: true,
                        confirmButtonText: 'Continuar comprando',
                        showCancelButton: true,
                        cancelButtonText: 'Ver carrito',
                        confirmButtonColor: '#d4825c',
                        cancelButtonColor: '#6c757d'
                    }).then((result) => {
                        if (!result.isConfirmed) {
                            window.location.href = '/Carrito/Carrito';
                        } else {
                            inputCantidad.value = 1;
                        }
                    });
                }
            } else {
                // Manejar errores retornados por el servidor
                manejarErrorAgregarCarrito(data.mensaje);
            }
        })
        .catch(error => {

            if (error.message === 'NO_AUTH') {
                manejarErrorAgregarCarrito('Debe iniciar sesión para agregar productos al carrito');
            } else {
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error de conexión',
                        text: 'No se pudo conectar con el servidor. Por favor, verifica tu conexión e intenta nuevamente.',
                        confirmButtonColor: '#d4825c'
                    });
                } else {
                    alert('Error de conexión. Por favor, intenta nuevamente.');
                }
            }
        });
}

// Manejar errores al agregar al carrito
function manejarErrorAgregarCarrito(mensaje) {

    // Si el mensaje indica que debe iniciar sesión
    if (mensaje && (mensaje.toLowerCase().includes('iniciar sesión') || mensaje.toLowerCase().includes('debe iniciar'))) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'warning',
                title: 'Inicia sesión',
                text: 'Debes iniciar sesión para agregar productos al carrito',
                showCancelButton: true,
                confirmButtonText: 'Ir a Login',
                cancelButtonText: 'Cancelar',
                confirmButtonColor: '#d4825c',
                cancelButtonColor: '#6c757d'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Guardar URL actual para redirigir después del login
                    sessionStorage.setItem('returnUrl', window.location.pathname);
                    window.location.href = '/Account/Login';
                }
            });
        } else {
            if (confirm('Debes iniciar sesión para agregar productos al carrito. ¿Ir a Login?')) {
                sessionStorage.setItem('returnUrl', window.location.pathname);
                window.location.href = '/Account/Login';
            }
        }
    } else {
        // Otros errores
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: mensaje || 'No se pudo agregar el producto al carrito',
                confirmButtonColor: '#d4825c'
            });
        } else {
            alert(mensaje || 'No se pudo agregar el producto');
        }
    }
}

// Actualizar contador del carrito en el navbar
function actualizarContadorCarrito(cantidadTotal) {
    const cartCount = document.querySelector('.cart-count');
    if (cartCount) {
        cartCount.textContent = cantidadTotal || 0;

        if (cantidadTotal > 0) {
            cartCount.style.display = 'flex';
        }

        // Animación del contador
        cartCount.style.transform = 'scale(1.3)';
        cartCount.style.transition = 'transform 0.3s ease';
        setTimeout(function () {
            cartCount.style.transform = 'scale(1)';
        }, 300);
    }
}

// Actualizar contador del carrito desde el servidor
function actualizarContadorCarritoDesdeServidor() {
    fetch('/Carrito/ObtenerCantidadProductos')
        .then(response => response.json())
        .then(data => {
            const cartCount = document.querySelector('.cart-count');
            if (cartCount) {
                const cantidad = data.cantidad || 0;
                cartCount.textContent = cantidad;

                if (cantidad > 0) {
                    cartCount.style.display = 'flex';
                } else {
                    cartCount.style.display = 'none';
                }
            }
        })
        .catch(error => {
            console.error('Error al obtener cantidad del carrito:', error);
        });
}

// Función auxiliar para mostrar mensajes
function mostrarMensaje(mensaje, tipo) {
    if (typeof Swal !== 'undefined') {
        const iconos = {
            success: 'success',
            error: 'error',
            warning: 'warning',
            info: 'info'
        };

        Swal.fire({
            icon: iconos[tipo] || 'info',
            title: mensaje,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    } else {
        alert(mensaje);
    }
}

// Función auxiliar para formatear precios
function formatearPrecio(precio) {
    return '₡' + precio.toLocaleString('es-CR', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    });
}

// Función para lazy loading de imágenes
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