var ViewModel = function () {
    var self = this;

    self.Games = ko.observableArray();
};

$(document).ready(function () {
    var model = new ViewModel();
    ko.applyBindings(model);

    $.ajax({
        url: '/SevenWonders/GetGameData',
        dataType: 'json',
        success: function (data) {
            if (data.Success) {
                model.Games($.map(data.Payload.games, function (g) {
                    return new GameModel(g);
                }));
            }
        }
    });
});