function formatCurrencyVND(amount) {
    return amount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}

function qnt_incre_cart_page() {
    var qtyBtns = document.querySelectorAll(".qty-cart-page.qtyBtn");

    qtyBtns.forEach(function (btn) {
        btn.addEventListener("click", function () {
            var qtyField = this.parentElement;
            var oldValue = parseInt(qtyField.querySelector(".qty").value);
            var newVal = 1;

            if (this.classList.contains("plus")) {
                newVal = oldValue + 1;
            } else if (oldValue > 1) {
                newVal = oldValue - 1;
            }

            qtyField.querySelector(".qty").value = newVal;

            const id = btn.getAttribute("data-id");
            const quantity = newVal;

            const payload = {
                'id': parseInt(id),
                'quantity': quantity
            }

            updateCartServerPage(payload);
        });
    });
}

async function updateCartServerPage(payload) {
    try {
        const response = await fetch("/Cart/Update", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error('Cập nhật giỏ hàng thất bại');
        }

        let cartResponse = await response.json();
        console.log('Cập nhật giỏ hàng thành công:', cartResponse);
        renderCartPage(cartResponse)
        renderCart(cartResponse)
    } catch (error) {
        console.error('Lỗi khi cập nhật giỏ hàng:', error.message);
    }
}

function renderCartPage(data) {
    const bodyCart = document.querySelector('#body-cart');

    if (!bodyCart) return;

    bodyCart.innerHTML = '';

    data.cartItems.forEach(item => {
        let html = `
                        <tr class="cart__row border-bottom line1 cart-flex border-top">
                                                <td class="cart__image-wrapper cart-flex-item">
                                                        <a href="#"><img class="cart__image" src="${item.productVariant.images[0].url}" alt="Elastic Waist Dress - Navy / Small"></a>
                                                </td>
                                                <td class="cart__meta small--text-left cart-flex-item">
                                                    <div class="list-view-item__title">
                                                            <a href="#">${item.productVariant.name}</a>
                                                    </div>

                                                    <div class="cart__meta-text">
                                                        Color: ${item.productVariant.color.name} <br>Size: ${item.productVariant.size.eSize}<br>
                                                    </div>
                                                </td>
                                                <td class="cart__price-wrapper cart-flex-item">
                                                        <span class="money">${formatCurrencyVND(item.productVariant.price)}</span>
                                                </td>
                                                <td class="cart__update-wrapper cart-flex-item text-right">
                                                    <div class="cart__qty text-center">
                                                        <div class="qtyField">
                                                                <a class="qty-cart-page qtyBtn minus" data-id="${item.productVariantId}" href="javascript:void(0);"><i class="icon icon-minus"></i></a>
                                                                    <input class="cart__qty-input qty" data-id="${item.productVariantId}" type="text" name="updates[]" id="qty" value="${item.quantity}" pattern="[0-9]*">
                                                                    <a class="qty-cart-page qtyBtn plus" data-id="${item.productVariantId}" href="javascript:void(0);"><i class="icon icon-plus"></i></a>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td class="text-right small--hide cart-price">
                                                        <div><span class="money">$ ${formatCurrencyVND(item.subTotal)}</span></div>
                                                </td>
                                                <td class="text-center small--hide"><a href="#" class="btn btn--secondary cart__remove" title="Remove tem"><i class="icon icon anm anm-times-l"></i></a></td>
                                            </tr>
                `;

        bodyCart.innerHTML += html;
    });

    const totalPricePage = document.getElementById('total-price-page')
    document.getElementById('total-price-popup').innerHTML = data.totalPrice

    if (totalPricePage) {
        totalPricePage.innerText = data.totalPrice
    }

    qnt_incre_cart_page();
}


// =================================== POPUP=============================
function qnt_incre() {
    var qtyBtns = document.querySelectorAll(".qty-cart-popup.qtyBtn");
    qtyBtns.forEach(function (btn) {
        btn.addEventListener("click", function () {
            var qtyField = this.parentElement;
            var oldValue = parseInt(qtyField.querySelector(".qty").value);
            var newVal = 1;

            if (this.classList.contains("plus")) {
                newVal = oldValue + 1;
            } else if (oldValue > 1) {
                newVal = oldValue - 1;
            }

            qtyField.querySelector(".qty").value = newVal;

            const id = btn.getAttribute("data-id");
            const quantity = newVal;

            const payload = {
                'id': parseInt(id),
                'quantity': quantity
            }

            updateCartServer(payload);
        });
    });
}


// =================================== QUICK VIEW=============================
function qnt_incre_quick_view() {
    var qtyBtns = document.querySelectorAll(".quick-view-qty.qtyBtn");
    console.log(qtyBtns)

    qtyBtns.forEach(function (btn) {
        btn.addEventListener("click", function () {
            var qtyField = this.parentElement;
            var oldValue = parseInt(qtyField.querySelector(".qty").value);
            var newVal = 1;

            if (this.classList.contains("plus")) {
                newVal = oldValue + 1;
            } else if (oldValue > 1) {
                newVal = oldValue - 1;
            }

            qtyField.querySelector(".qty").value = newVal;

        });
    });
}

async function updateCartServer(payload) {
    try {
        const response = await fetch("/Cart/Update", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error('Cập nhật giỏ hàng thất bại');
        }

        const data = await response.json();
        console.log('Cập nhật giỏ hàng thành công:', data);
        renderCartPage(data)
        renderCart(data)
    } catch (error) {
        console.error('Lỗi khi cập nhật giỏ hàng:', error.message);
    }
}

function renderCart(data) {
    const cartList = document.querySelector('.mini-products-list');
    cartList.innerHTML = '';

    data.cartItems.forEach(item => {
        const listItem = document.createElement('li');
        listItem.classList.add('item');

        listItem.innerHTML = `
                <a class="product-image" href="#">
                    <img src="${item.productVariant.images[0].url}" alt="${item.productVariant.name}" title="" />
                </a>
                <div class="product-details">
                    <a href="#" class="remove"><i class="anm anm-times-l" aria-hidden="true"></i></a>
                    <a href="#" class="edit-i remove"><i class="anm anm-edit" aria-hidden="true"></i></a>
                    <a class="pName" href="cart.html">${item.productVariant.name}</a>
                    <div class="variant-cart">${item.productVariant.color.name} / ${item.productVariant.size.eSize}</div>
                    <div class="wrapQtyBtn">
                        <div class="qtyField">
                            <span class="label">Qty:</span>
                                <a class="qty-cart-popup qtyBtn minus" data-id="${item.productVariantId}" href="javascript:void(0);"><i class="fa anm anm-minus-r" aria-hidden="true"></i></a>
                            <input type="text" id="Quantity" name="quantity" value="${item.quantity}" data-id="${item.productVariantId}" class="product-form__input qty">
                                <a class="qty-cart-popup qtyBtn plus" data-id="${item.productVariantId}" href="javascript:void(0);"><i class="fa anm anm-plus-r" aria-hidden="true"></i></a>
                        </div>
                    </div>
                    <div class="priceRow">
                        <div class="product-price">
                            <span class="money">$${formatCurrencyVND(item.subTotal)}</span>
                        </div>
                    </div>
                </div>
            `;

        cartList.appendChild(listItem);
    });

    const totalPricePage = document.getElementById('total-price-page')

    document.getElementById('total-price-popup').innerHTML = formatCurrencyVND(data.totalPrice)

    if (totalPricePage) {
        totalPricePage.innerText = formatCurrencyVND(data.totalPrice)
    }

    qnt_incre()
}

//=========================PRODUCT DETAIL=======================

function qnt_incre_product_detail() {
    var qtyBtns = document.querySelectorAll(".product-detail.qtyBtn");
    console.log(qtyBtns)

    qtyBtns.forEach(function (btn) {
        btn.addEventListener("click", function () {
            var qtyField = this.parentElement;
            var oldValue = parseInt(qtyField.querySelector(".qty").value);
            var newVal = 1;

            if (this.classList.contains("plus")) {
                newVal = oldValue + 1;
            } else if (oldValue > 1) {
                newVal = oldValue - 1;
            }

            qtyField.querySelector(".qty").value = newVal;

            const id = btn.getAttribute("data-id");
            const quantity = newVal;

            const payload = {
                'id': parseInt(id),
                'quantity': quantity
            }

        });
    });
}

//=========CALL===========
document.addEventListener("DOMContentLoaded", function () {
    qnt_incre();
    qnt_incre_cart_page();
    qnt_incre_product_detail();
    qnt_incre_quick_view();
});