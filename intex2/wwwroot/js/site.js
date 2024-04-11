﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function(){
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


$('.product-type').on('change', function() {
    var selectedProductTypes = $('.product-type:checked').map(function() {
        return this.value;
    }).get();

    $.ajax({
        url: '/Home/Shop',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ productTypes: selectedProductTypes }),
        success: function(data) {
            console.log('Data returned from server:', data);
            $('#products-section').html(data);
        }
    });
});