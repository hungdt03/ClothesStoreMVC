const btnSubmitCart = document.getElementById('btn-add-to-cart')
document.addEventListener('DOMContentLoaded', function () {
    const colorInputs = document.querySelectorAll('input[name="colorId"]');
    colorInputs.forEach(colorInput => {
        colorInput.addEventListener('change', handleInputCartPageChange)
    })

});

const handleInputCartPageChange = async (e) => {
    
    var skuCodeTag = document.getElementById('product-skucode')
    var selectedColor = document.querySelector('input[name="colorId"]:checked');
    var selectedSize = document.querySelector('input[name="sizeId"]:checked');

    const selectColorTag = document.getElementById('selectColor');
    const selectSizeTag = document.getElementById('selectSize');

    if (selectedSize) {
        selectSizeTag.innerText = selectedSize.getAttribute('data-name')
    }

    if (selectedColor && selectedSize) {
        selectColorTag.innerText = selectedColor.getAttribute('data-name')
        btnSubmitCart.disabled = false;
        return;
    } else {
        btnSubmitCart.disabled = true;
    }
    

    if (selectedColor) {
        selectColorTag.innerText = selectedColor.getAttribute('data-name')
        var colorValue = selectedColor.value;
      
        const payload = {
            'colorId': parseInt(colorValue),
            'productId': parseInt(skuCodeTag.innerText.trim())
        }
        
        const response = await fetch('/Product/GetSizesByColor', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })

        const responseData = await response.json()
        render(responseData)

    } 
    
}


function render(data) {
    var wrapperSize = document.getElementById('wrapper-size')
    const quickViewProductImage = document.getElementById('quickview-product-img')
    quickViewProductImage.src = data.images[0].url

    wrapperSize.innerHTML = '<label class="header">Size: <span id="selectSize" class="slVariant"></span></label>';

    data.sizes.forEach(item => {
        let html = `
            <div data-value="${item.eSize}" class="swatch-element xs available">
                <input class="swatchInput" id="swatch-1-${item.eSize}" type="radio" data-name=${item.eSize} name="sizeId" value="${item.id}">
                <label class="swatchLbl small flat" for="swatch-1-${item.eSize}" title="${item.eSize}">${item.eSize}</label>
            </div>
        `

        wrapperSize.innerHTML += html;
    })

    const sizeInputs = document.querySelectorAll('input[name="sizeId"]');
    sizeInputs.forEach(sizeInput => {
        sizeInput.addEventListener('change', handleInputCartPageChange)
    })
}
