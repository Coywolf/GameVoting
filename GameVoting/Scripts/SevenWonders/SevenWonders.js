var ViewModel = function () {
    var self = this;

    self.Games = ko.observableArray();
    self.Users = ko.observableArray();
    self.Boards = ko.observableArray();

    self.SelectedGame = ko.observable();

    self.CreatingGame = ko.pureComputed(function () {
        return self.SelectedGame() && !self.SelectedGame().GameId;
    });

    self.NewGame = function () {
        self.SelectedGame(new GameModel());
    };

    self.CreateGame = function () {
        // todo validate
        $.ajax({
            url: '/SevenWonders/CreateGame',
            type: 'POST',
            dataType: 'json',
            data: {
                data: ko.toJSON(self.SelectedGame())
            },
            success: function (data) {
                if (data.Success) {
                    var game = new GameModel(data.Payload);
                    self.Games.unshift(game);
                    self.SelectedGame(game);
                }
                else {
                    // todo error
                }
            }
        });
    };

    self.CancelGame = function () {
        self.SelectedGame(null);
    };
};

var GameModel = function (data) {
    data = data || {};
    var self = this;

    self.GameId = data.GameId;
    self.CreatedDate = (data.CreatedDate ? new Date(data.CreatedDate) : new Date()).toLocaleDateString();
    self.Creator = data.Creator;

    self.Players = ko.observableArray(data.Players == undefined ? null :
        $.map(data.Players, function (p) {
            return new PlayerModel(p);
        })
    );

    // existing games use the readonly template
    self.rowTemplate = data.GameId == undefined ?
        "sevenwonders-scoresheet-editrow-template" :
        "sevenwonders-scoresheet-viewrow-template";

    self.CanAddPlayer = ko.pureComputed(function () {
        return data.GameId == undefined && self.Players().length < 8;
    });

    self.AddPlayer = function () {
        self.Players.push(new PlayerModel());
    };
}

var PlayerModel = function (data) {
    data = data || {};
    var self = this;

    self.UserId = ko.observable(data.UserId);
    self.UserId.subscribe(function (newValue) {
        if (newValue && isNaN(newValue)) {
            self.UserId(null);
            self.Name = newValue;
        }
    });

    self.BoardId = ko.observable(data.BoardId);

    self.Name = data.Name;
    self.BoardName = data.BoardName;

    self.MilitaryScore = ko.observable(data.MilitaryScore).extend({numeric: 0});
    self.CoinScore = ko.observable(data.CoinScore).extend({ numeric: 0 });
    self.WonderScore = ko.observable(data.WonderScore).extend({ numeric: 0 });
    self.CivicScore = ko.observable(data.CivicScore).extend({ numeric: 0 });
    self.CommercialScore = ko.observable(data.CommercialScore).extend({ numeric: 0 });
    self.GuildScore = ko.observable(data.GuildScore).extend({ numeric: 0 });
    self.ScienceScore = ko.observable(data.ScienceScore).extend({ numeric: 0 });
    self.LeaderScore = ko.observable(data.LeaderScore).extend({ numeric: 0 });

    self.TotalScore = ko.pureComputed(function () {
        return self.MilitaryScore() +
            self.CoinScore() +
            self.WonderScore() +
            self.CivicScore() +
            self.CommercialScore() +
            self.GuildScore() +
            self.ScienceScore() +
            self.LeaderScore()
    });
}

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
                model.Users($.map(data.Payload.users, function (u) {
                    return {
                        id: u.UserId,
                        name: u.UserName
                    };
                }));
                model.Boards($.map(data.Payload.boards, function (b) {
                    return {
                        id: b.BoardId,
                        name: b.Name
                    };
                }));
            }
        }
    });
});

ko.extenders.numeric = function (target, precision) {
    //create a writable computed observable to intercept writes to our observable
    var result = ko.pureComputed({
        read: target,  //always return the original observables value
        write: function (newValue) {
            var current = target(),
                roundingMultiplier = Math.pow(10, precision),
                newValueAsNum = isNaN(newValue) ? 0 : parseFloat(+newValue),
                valueToWrite = Math.round(newValueAsNum * roundingMultiplier) / roundingMultiplier;

            //only write if it changed
            if (valueToWrite !== current) {
                target(valueToWrite);
            } else {
                //if the rounded value is the same, but a different value was written, force a notification for the current field
                if (newValue !== current) {
                    target.notifySubscribers(valueToWrite);
                }
            }
        }
    }).extend({ notify: 'always' });

    //initialize with current value to make sure it is rounded appropriately
    result(target());

    //return the new computed observable
    return result;
};

ko.bindingHandlers.select2 = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();

        var options = {
            width: '100%',
            placeholder: allBindings.get('optionsCaption')
        };
        options = $.extend(true, options, allBindings.get('select2options'));

        var selector = $(element);
        selector.select2(options);
        selector.on("select2:select", function (e) {
            if (selector.attr('multiple')) {
                value.push(e.params.data.id);
            }
            else {
                value(e.params.data.id);
            }
        });
        selector.on("select2:unselect", function (e) {
            if (selector.attr('multiple')) {
                value.remove(e.params.data.id);
            }
            else {
                value(null);
            }
        });
    },
    after: ['options']
};