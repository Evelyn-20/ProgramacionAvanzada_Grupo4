// carrito.js - JavaScript para la gestión del carrito de compras

document.addEventListener('DOMContentLoaded', function () {
    inicializarBotonesCarrito();
    inicializarFormularioFinalizacion();
});

// Inicializar efectos hover de los botones del carrito
function inicializarBotonesCarrito() {
    // Botones de cantidad (+/-)
    const botonesCantidad = document.querySelectorAll('.btn-cantidad');
    botonesCantidad.forEach(btn => {
        btn.addEventListener('mouseenter', function () {
            this.style.background = 'var(--primary-color)';
            this.style.color = 'var(--white)';
            this.style.transform = 'scale(1.1)';
        });
        btn.addEventListener('mouseleave', function () {
            this.style.background = 'var(--secondary-color)';
            this.style.color = 'var(--dark-color)';
            this.style.transform = 'scale(1)';
        });
    });

    // Botones de eliminar producto
    const botonesEliminar = document.querySelectorAll('.btn-eliminar-producto');
    botonesEliminar.forEach(btn => {
        btn.addEventListener('mouseenter', function () {
            this.style.background = '#c0392b';
            this.style.transform = 'scale(1.1)';
        });
        btn.addEventListener('mouseleave', function () {
            this.style.background = '#e74c3c';
            this.style.transform = 'scale(1)';
        });
    });
}

// Inicializar funcionalidad del formulario de finalización de compra
function inicializarFormularioFinalizacion() {
    // Formateo automático del número de tarjeta
    const numeroTarjetaInput = document.querySelector('input[name="NumeroTarjeta"]');
    if (numeroTarjetaInput) {
        numeroTarjetaInput.addEventListener('input', function (e) {
            let valor = e.target.value.replace(/\s/g, '');
            let valorFormateado = valor.match(/.{1,4}/g);
            e.target.value = valorFormateado ? valorFormateado.join(' ') : valor;
        });

        // Validar solo números
        numeroTarjetaInput.addEventListener('keypress', function (e) {
            if (!/[0-9]/.test(e.key) && e.key !== 'Backspace' && e.key !== 'Delete') {
                e.preventDefault();
            }
        });
    }

    // Validación del CVV (solo números)
    const cvvInput = document.querySelector('input[name="CVV"]');
    if (cvvInput) {
        cvvInput.addEventListener('keypress', function (e) {
            if (!/[0-9]/.test(e.key) && e.key !== 'Backspace' && e.key !== 'Delete') {
                e.preventDefault();
            }
        });
    }

    // Formateo del teléfono
    const telefonoInput = document.querySelector('input[name="Telefono"]');
    if (telefonoInput) {
        telefonoInput.addEventListener('input', function (e) {
            let valor = e.target.value.replace(/\D/g, '');
            if (valor.length > 8) {
                valor = valor.substring(0, 8);
            }
            if (valor.length > 4) {
                e.target.value = valor.substring(0, 4) + '-' + valor.substring(4);
            } else {
                e.target.value = valor;
            }
        });
    }

    // Validación del formulario antes de enviar
    const formularioPago = document.querySelector('form[action*="ProcesarPago"]');
    if (formularioPago) {
        formularioPago.addEventListener('submit', function (e) {
            if (!validarFormularioPago()) {
                e.preventDefault();
                return false;
            }
        });
    }
}

// Validar formulario de pago
function validarFormularioPago() {
    let errores = [];

    // Validar número de tarjeta
    const numeroTarjeta = document.querySelector('input[name="NumeroTarjeta"]');
    if (numeroTarjeta) {
        const numero = numeroTarjeta.value.replace(/\s/g, '');
        if (numero.length < 13 || numero.length > 19) {
            errores.push('El número de tarjeta debe tener entre 13 y 19 dígitos');
        }
    }

    // Validar CVV
    const cvv = document.querySelector('input[name="CVV"]');
    if (cvv && cvv.value.length !== 3) {
        errores.push('El CVV debe tener 3 dígitos');
    }

    // Validar fecha de vencimiento
    const mes = document.querySelector('select[name="MesVencimiento"]');
    const anio = document.querySelector('select[name="AñoVencimiento"]');
    if (mes && anio) {
        if (!mes.value || !anio.value) {
            errores.push('Debe seleccionar el mes y año de vencimiento');
        } else {
            const fechaVencimiento = new Date(parseInt(anio.value), parseInt(mes.value) - 1);
            const fechaActual = new Date();
            if (fechaVencimiento < fechaActual) {
                errores.push('La tarjeta está vencida');
            }
        }
    }

    // Validar email
    const email = document.querySelector('input[name="Email"]');
    if (email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email.value)) {
            errores.push('El correo electrónico no es válido');
        }
    }

    // Validar teléfono
    const telefono = document.querySelector('input[name="Telefono"]');
    if (telefono) {
        const telefonoLimpio = telefono.value.replace(/\D/g, '');
        if (telefonoLimpio.length < 8) {
            errores.push('El teléfono debe tener al menos 8 dígitos');
        }
    }

    // Mostrar errores si existen
    if (errores.length > 0) {
        alert('Por favor, corrija los siguientes errores:\n\n' + errores.join('\n'));
        return false;
    }

    return true;
}

// Función para actualizar el total del carrito (puede ser usada dinámicamente)
function actualizarTotalCarrito() {
    const productosCarrito = document.querySelectorAll('.producto-carrito');
    let subtotal = 0;

    productosCarrito.forEach(producto => {
        const precioElement = producto.querySelector('.precio-producto');
        const cantidadElement = producto.querySelector('input[name="Cantidad"]');

        if (precioElement && cantidadElement) {
            const precio = parseFloat(precioElement.dataset.precio || 0);
            const cantidad = parseInt(cantidadElement.value || 0);
            subtotal += precio * cantidad;
        }
    });

    // Actualizar visualización si existe el elemento
    const subtotalElement = document.getElementById('subtotal-carrito');
    if (subtotalElement) {
        subtotalElement.textContent = '₡' + subtotal.toLocaleString('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    return subtotal;
}

// Animación de carga al procesar pago
function mostrarCargandoPago(boton) {
    if (boton) {
        boton.disabled = true;
        boton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Procesando...';
        boton.style.opacity = '0.7';
        boton.style.cursor = 'not-allowed';
    }
}