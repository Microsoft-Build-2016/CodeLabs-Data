function htmlEncode(value){
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
};

function htmlDecode(value){
    return $('<div/>').html(value).text();
};

function deleteSize(sizeText, sizeId) {
    //  remove the existing html row
    $("#ProductSizeRow" + sizeId).remove();

    // update the hidden array
    var currentSizes = $("#ProductSizes").val();
    var newSizes = [];
    var sizes = currentSizes.split("|");
    for (var i = 0; i < sizes.length; i++) {
        if (sizes[i] != sizeText && sizes[i].length > 0) {
            newSizes.push(sizes[i]);
        }
    }
    $("#ProductSizes").val(newSizes.join("|"));
};

function addSize() {
    var newSize = $("#AddSize").val();
    if (newSize) {
        newSize = htmlEncode(newSize);
        var sizes = $("#ProductSizes").val();
        sizes = sizes + "|" + newSize;
        $("#ProductSizes").val(sizes);
        $("#AddSize").val("");
        renderSize(newSize);
        //alert("Sizes is now: " + sizes);
    }
};

function renderSizes(){
    var sizes = $("#ProductSizes").val().split("|");
    sizes.forEach(function (size) {
        if (size.length) {
            renderSize(size);
        }
    });
};

function renderSize(size) {
    var sizeEnc = htmlEncode(size).replace("\"", "&quot;");
    var sizeId = size.replace("&quot;", "").replace("\"", "");
    var sizeHtml = "<div class=\"row\" id=\"ProductSizeRow" + sizeId + "\">" +
        "<div class=\"col-md-1\">" + size + "</div>" +
        "<div class=\"col-md-1\">" +
        "<a onclick=\"deleteSize('" + sizeEnc + "', '" + sizeId + "');\">Delete</a>" +
        "</div></div>";
    $("#ProductSizeHtml").append(sizeHtml);
};

$(document).ready(function() {
    renderSizes();
});