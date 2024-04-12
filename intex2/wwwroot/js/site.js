// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function(){
    // Check if the productType query parameter is "star wars"
    var urlParams = new URLSearchParams(window.location.search);
    var productType = urlParams.get('productType');
    if (productType && productType.toLowerCase() === 'star wars') {
        // Check the "Star Wars" filter box
        $('#star-wars').prop('checked', true);
    }
    $("#seeMore").click(function (e) {
        e.preventDefault(); // Prevent the default anchor click action
        $("#shortDescription").hide(); // Hide the short description
        $("#fullDescription").show(); // Show the full description
    });

    $("#seeLess").click(function(e){
        e.preventDefault(); // Prevent the default anchor click action
        $("#fullDescription").hide(); // Hide the full description
        $("#shortDescription").show(); // Show the short description
    });
});


$('input[type=checkbox]').on('change', function() {
    var selectedProductTypes = $('.product-type:checked').map(function() {
        return this.value;
    }).get();

    var selectedColors = $('input[name="colors"]:checked').map(function() {
        return this.value;
    }).get();

    var url = '/Home/Shop';
    var data = JSON.stringify({ productTypes: selectedProductTypes, colors: selectedColors });

    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: data,
        success: function(data) {
            console.log('Data returned from server:', data);
            $('#products-section').html(data);
        }
    });
});


