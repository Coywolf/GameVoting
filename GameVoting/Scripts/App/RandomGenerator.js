$(document).ready(function() {
    $('#randomGenerator').popover({
        content: function () {
            return $('#randomGeneratorContent').html();
        },
        html: true,
        placement: 'bottom'
    });
    $('#randomGenerator').on('shown.bs.popover', function () {
        var isNormalInteger = function(str) {
            var n = ~~Number(str);
            return String(n) === str && n >= 0;
        };
        var generate = function() {
            var valid = true;
            var amount = $('#randomAmount').val();
            var from = $('#randomFrom').val();
            var to = $('#randomTo').val();

            if (isNormalInteger(amount)) {
                $('#randomAmount').parent().removeClass('has-error');
            } else {
                $('#randomAmount').parent().addClass('has-error');
                valid = false;
            }
            if (isNormalInteger(from)) {
                $('#randomFrom').parent().removeClass('has-error');
            } else {
                $('#randomFrom').parent().addClass('has-error');
                valid = false;
            }
            if (isNormalInteger(to)) {
                $('#randomTo').parent().removeClass('has-error');
            } else {
                $('#randomTo').parent().addClass('has-error');
                valid = false;
            }

            if (valid) {
                amount = parseInt(amount);
                from = parseInt(from);
                to = parseInt(to);

                $("#randomResults").html("");
                for (var i = 0; i < amount; i++) {
                    var number = Math.floor(Math.random() * (to - from)) + from;
                    $('<li>' + number + '</li>').appendTo("#randomResults");
                }
            }
        };
        
        $('#randomAmount').select();
        $('#randomGo').on('click', generate);
        $('#randomAmount, #randomFrom, #randomTo').on('keyup', function(e) {
            if(e.which == 13 || e.keypress == 13) {
                return generate();
            }
        });
    });
});