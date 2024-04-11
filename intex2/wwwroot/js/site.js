// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
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

// Function to send the selected product types and colors to the server
function sendFilters() {
    var selectedProductTypes = $('.product-type:checked').map(function() {
        return this.value;
    }).get();

    var selectedColors = $('input[name="colors"]:checked').map(function() {
        return this.value;
    }).get();

    var url = '/Home/Shop';
    var data = JSON.stringify({ productTypes: selectedProductTypes, colors: selectedColors });

    //Update the hidden inputs for filters
    $('input[name="productTypes"]').val(selectedProductTypes);
    $('input[name="colors"]').val(selectedColors);
    
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
}

// Call sendFilters when a checkbox is changed
$('input[type=checkbox]').on('change', sendFilters);

// Call sendFilters when the page size is changed
$('select[name="pageSize"]').on('change', sendFilters);

function updateFormAction(selectElement) {
    var form = selectElement.form;
    var pageSize = selectElement.value;
    var productTypes = $('.product-type:checked').map(function() { return this.value; }).get();
    var colors = $('input[name="colors"]:checked').map(function() { return this.value; }).get();

    var actionUrl = '/Home/Shop?pageNum=1&pageSize=' + pageSize;
    productTypes.forEach(function(pt) { actionUrl += '&productTypes=' + encodeURIComponent(pt); });
    colors.forEach(function(color) { actionUrl += '&colors=' + encodeURIComponent(color); });

    form.action = actionUrl;
    form.submit();
}

function updatePageSizeAction(form) {
    var currentUrl = window.location.href;
    var newPageSize = form.pageSize.value;

    // Remove existing pageSize parameter from the URL if it exists
    var updatedUrl = new URL(currentUrl);
    updatedUrl.searchParams.delete('pageSize'); // Remove existing pageSize, if any
    updatedUrl.searchParams.set('pageSize', newPageSize); // Set new pageSize

    form.action = updatedUrl.toString(); // Update form action
    return true; // Return true to allow the form submission to proceed
}

