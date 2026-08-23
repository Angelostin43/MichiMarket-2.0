function playMeow(e) {
    e.preventDefault();
    var audio = document.getElementById('meowSound');
    var href = e.currentTarget.href;
    if (audio) {
        audio.currentTime = 0;
        audio.play().then(function() {
            setTimeout(function() { window.location.href = href; }, 800);
        }).catch(function() {
            window.location.href = href;
        });
    } else {
        window.location.href = href;
    }
}

function abrirLogin() {
    document.getElementById('loginModal').classList.add('active');
}

function cerrarLogin() {
    document.getElementById('loginModal').classList.remove('active');
}

var loginModal = document.getElementById('loginModal');
if (loginModal) {
    loginModal.addEventListener('click', function (e) {
        if (e.target === this) {
            cerrarLogin();
        }
    });
}

(function () {
    var tokenEl = document.querySelector('#antiforgery-token-container input[name="__RequestVerificationToken"]');
    if (tokenEl && typeof $ !== 'undefined') {
        $.ajaxSetup({
            beforeSend: function (xhr) {
                xhr.setRequestHeader('X-CSRF-TOKEN', tokenEl.value);
            }
        });
    }
})();

function fusionarCarritoLocal() {
    var cart = JSON.parse(localStorage.getItem('carrito') || '[]');
    if (cart.length > 0) {
        var tkn = document.querySelector('#antiforgery-token-container input[name="__RequestVerificationToken"]');
        fetch('/Carrito/FusionarCarrito', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': tkn ? tkn.value : ''
            },
            body: JSON.stringify(cart)
        }).then(function() {
            localStorage.removeItem('carrito');
        });
    }
}

function previewImage(e) {
    var file = e.target.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function(ev) {
            var img = document.getElementById('imgPreview');
            if (img) {
                img.src = ev.target.result;
                img.style.display = 'block';
            }
            var icon = document.querySelector('.img-preview-icon');
            if (icon) icon.style.display = 'none';
        };
        reader.readAsDataURL(file);
    }
}

function toggleAvanzado() {
    var panel = document.getElementById('busquedaAvanzada');
    var btn = document.getElementById('btnAvanzado');
    panel.classList.toggle('active');
    btn.classList.toggle('active');
}

function toggleCartPopup(e, id) {
    e.stopPropagation();
    var popup = document.getElementById('popup-' + id);
    document.querySelectorAll('.cart-popup.active').forEach(function(p) {
        if (p !== popup) p.classList.remove('active');
    });
    popup.classList.toggle('active');
}

document.addEventListener('click', function (e) {
    if (!e.target.closest('.cart-popup') && !e.target.closest('.btn-cart-bookmark')) {
        document.querySelectorAll('.cart-popup.active').forEach(function(p) { p.classList.remove('active'); });
    }
});

function getLocalCart() {
    return JSON.parse(localStorage.getItem('carrito') || '[]');
}

function saveLocalCart(cart) {
    localStorage.setItem('carrito', JSON.stringify(cart));
}

function marcarBotonesActivos() {
    var cart = getLocalCart();
    cart.forEach(function(item) {
        var btn = document.getElementById('btn-' + item.id_producto);
        if (btn) btn.classList.add('active');
    });
}

function agregarAlCarrito(idProducto) {
    var qty = parseInt(document.getElementById('qty-' + idProducto).value);
    var btn = document.getElementById('btn-' + idProducto);

    if (!window.estaLogueado) {
        var cart = getLocalCart();
        var existente = cart.find(function(i) { return i.id_producto === idProducto; });
        if (existente) {
            existente.cantidad += qty;
        } else {
            cart.push({
                id_producto: idProducto,
                nombre: btn.getAttribute('data-nombre'),
                precio: parseFloat(btn.getAttribute('data-precio')),
                tipo: btn.getAttribute('data-tipo'),
                animal: btn.getAttribute('data-animal'),
                foto: btn.getAttribute('data-foto'),
                cantidad: qty
            });
        }
        saveLocalCart(cart);
        btn.classList.add('active');
        document.getElementById('popup-' + idProducto).classList.remove('active');
        return;
    }

    var tkn = document.querySelector('#antiforgery-token-container input[name="__RequestVerificationToken"]');
    fetch('/Carrito/Agregar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': tkn ? tkn.value : ''
        },
        body: 'idProducto=' + idProducto + '&cantidad=' + qty
    })
    .then(function(r) { return r.json(); })
    .then(function(data) {
        if (data.ok) {
            btn.classList.add('active');
            document.getElementById('popup-' + idProducto).classList.remove('active');
        } else {
            window.location.href = data.redirect;
        }
    });
}

function agregarAlCarritoDetalle(idProducto) {
    var qty = parseInt(document.getElementById('qty-' + idProducto).value);
    var btn = document.getElementById('btn-' + idProducto);

    if (!window.estaLogueado) {
        var cart = JSON.parse(localStorage.getItem('carrito') || '[]');
        var existente = cart.find(function(i) { return i.id_producto === idProducto; });
        if (existente) {
            existente.cantidad += qty;
        } else {
            cart.push({
                id_producto: idProducto,
                nombre: btn.getAttribute('data-nombre'),
                precio: parseFloat(btn.getAttribute('data-precio')),
                tipo: btn.getAttribute('data-tipo'),
                animal: btn.getAttribute('data-animal'),
                foto: btn.getAttribute('data-foto'),
                cantidad: qty
            });
        }
        localStorage.setItem('carrito', JSON.stringify(cart));
        btn.classList.add('active');
        return;
    }

    var tkn = document.querySelector('#antiforgery-token-container input[name="__RequestVerificationToken"]');
    fetch('/Carrito/Agregar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': tkn ? tkn.value : ''
        },
        body: 'idProducto=' + idProducto + '&cantidad=' + qty
    })
    .then(function(r) { return r.json(); })
    .then(function(data) {
        if (data.ok) {
            btn.classList.add('active');
        } else {
            window.location.href = data.redirect;
        }
    });
}

function irAPagina(pagina) {
    if (window.esBusqueda) {
        document.getElementById('paginaField').value = pagina;
        document.getElementById('formBusqueda').submit();
    } else if (window.esAdmin) {
        window.location.href = '/Admin/Productos?pagina=' + pagina;
    } else {
        window.location.href = '/Producto?pagina=' + pagina;
    }
}

function toggleSelectAll(cb) {
    document.querySelectorAll('.product-checkbox').forEach(function(c) {
        c.checked = cb.checked;
    });
    updateBatchUI();
}

function updateBatchUI() {
    var checks = document.querySelectorAll('.product-checkbox:checked');
    var count = checks.length;
    var section = document.getElementById('batchDeleteSection');
    var counter = document.getElementById('selectedCount');
    if (count > 0) {
        section.classList.remove('d-none');
        counter.textContent = count;
    } else {
        section.classList.add('d-none');
    }
    var selectAll = document.getElementById('selectAll');
    var total = document.querySelectorAll('.product-checkbox').length;
    selectAll.checked = total > 0 && count === total;
}

function confirmarEliminarVarios() {
    var checks = document.querySelectorAll('.product-checkbox:checked');
    if (checks.length === 0) return;
    if (!confirm('Eliminar ' + checks.length + ' producto(s)?')) return;

    var ids = [];
    checks.forEach(function(c) { ids.push(c.value); });

    var form = document.createElement('form');
    form.method = 'POST';
    form.action = '/Admin/EliminarVarios';

    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'ids';
    input.value = ids.join(',');
    form.appendChild(input);

    document.body.appendChild(form);
    form.submit();
}

function eliminarLocal(idProducto) {
    var cart = getLocalCart().filter(function(i) { return i.id_producto !== idProducto; });
    saveLocalCart(cart);
    renderCarritoLocal();
}

function cambiarCantidadLocal(idProducto, delta) {
    var cart = getLocalCart();
    var item = cart.find(function(i) { return i.id_producto === idProducto; });
    if (!item) return;
    item.cantidad += delta;
    if (item.cantidad <= 0) {
        cart = cart.filter(function(i) { return i.id_producto !== idProducto; });
    }
    saveLocalCart(cart);
    renderCarritoLocal();
}

function renderCarritoLocal() {
    var cart = getLocalCart();
    var itemsDiv = document.getElementById('carritoLocalItems');
    var vacioDiv = document.getElementById('carritoLocalVacio');
    var contentDiv = document.getElementById('carritoLocalContent');

    if (!itemsDiv) return;

    if (cart.length === 0) {
        vacioDiv.style.display = 'block';
        contentDiv.style.display = 'none';
        return;
    }

    vacioDiv.style.display = 'none';
    contentDiv.style.display = 'block';

    var html = '';
    var total = 0;
    cart.forEach(function(item) {
        var sub = item.precio * item.cantidad;
        total += sub;
        var tipoUpper = item.tipo.charAt(0).toUpperCase() + item.tipo.slice(1);
        var animalUpper = item.animal.charAt(0).toUpperCase() + item.animal.slice(1);
        var minusDisabled = item.cantidad <= 1 ? 'disabled' : '';
        html += '<div class="carrito-item mb-3">' +
            '<div class="d-flex align-items-center gap-3">' +
            '<div class="carrito-img-wrapper"><img src="/' + item.foto + '" class="carrito-img" /></div>' +
            '<div class="flex-grow-1">' +
            '<h6 class="fw-bold mb-1">' + item.nombre + '</h6>' +
            '<p class="text-muted small mb-1"><span class="product-badge" style="font-size:10px;">' + tipoUpper + '</span> &middot; ' + animalUpper + '</p>' +
            '<p class="mb-0" style="color: var(--turquesa-dark); font-weight: 700;">S/ ' + item.precio.toFixed(2) + ' c/u</p>' +
            '</div>' +
            '<div class="d-flex align-items-center gap-2">' +
            '<button class="btn btn-sm btn-cart-qty" onclick="cambiarCantidadLocal(' + item.id_producto + ', -1)" ' + minusDisabled + '><i class="fas fa-minus"></i></button>' +
            '<span class="fw-bold" style="min-width:24px;text-align:center;">' + item.cantidad + '</span>' +
            '<button class="btn btn-sm btn-cart-qty" onclick="cambiarCantidadLocal(' + item.id_producto + ', 1)"><i class="fas fa-plus"></i></button>' +
            '</div>' +
            '<div class="text-end" style="min-width: 90px;">' +
            '<p class="fw-bold mb-2" style="color: var(--negro);">S/ ' + sub.toFixed(2) + '</p>' +
            '<button class="btn btn-sm btn-eliminar" onclick="eliminarLocal(' + item.id_producto + ')"><i class="fas fa-trash-alt"></i></button>' +
            '</div></div></div>';
    });
    itemsDiv.innerHTML = html;
    document.getElementById('localCount').textContent = 'Productos (' + cart.length + ')';
    document.getElementById('localSubtotal').textContent = 'S/ ' + total.toFixed(2);
    document.getElementById('localTotal').textContent = 'S/ ' + total.toFixed(2);
}
