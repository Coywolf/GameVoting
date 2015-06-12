var ViewModel = function () {
    var self = this;

    self.Users = ko.observableArray();
    self.Boards = ko.observableArray();

    self.Game = new GameModel();

    self.WizardModel = ko.observable();

    // todo single select
    self.MemberListInit = function (elements, data) {
        //i hate this if check, there has to be a more elegant way
        //but i need this to only happen after everything is rendered. +1 for the blank option
        if ($('#member-select').children().length == self.Users().length + 1) {
            var memberElement = $('#member-select').first();
            memberElement.chosen({
                width: "100%",
                placeholder_text_multiple: "Members (Optional)"
            });
            //change event to update the Members observable
            memberElement.on('change', function (ev, params) {
                if (params.selected) {
                    //self.NewEvent().Members.push(params.selected);
                }
                if (params.deselected) {
                    //self.NewEvent().Members.remove(params.deselected);
                }
            });
        }
    };

    // todo single select, boards
    self.BoardListInit = function (elements, data) {
        //i hate this if check, there has to be a more elegant way
        //but i need this to only happen after everything is rendered. +1 for the blank option
        if ($('#member-select').children().length == self.Users().length + 1) {
            var memberElement = $('#member-select').first();
            memberElement.chosen({
                width: "100%",
                placeholder_text_multiple: "Boards (Optional)"
            });
            //change event to update the Members observable
            memberElement.on('change', function (ev, params) {
                if (params.selected) {
                    //self.NewEvent().Members.push(params.selected);
                }
                if (params.deselected) {
                    //self.NewEvent().Members.remove(params.deselected);
                }
            });
        }
    };

    self.CreateGame = function () {
        // submit the new game
    };

    var ScoreProperties = ["MilitaryScore", "CoinScore", "WonderScore", "CivicScore", "CommercialScore", "GuildScore", "ScienceScore", "LeaderScore"];
    var currentProperty = 0;

    // advance to the next step of the calculator
    self.NextStep = function (nextPhase) {
        if (self.WizardModel().Template == "sevenwonders-new-player-template") {
            if (nextPhase || self.Game.Players.length >= 7) {
                // move to scores
                
            }

            // adding players
            var newPlayer = new PlayerModel();
            self.Game.Players.push(newPlayer);

            self.WizardModel(new WizardModel("sevenwonders-new-player-template", new PlayerWizardModel(newPlayer, self.Game.Players.length)));
        }
        else {
            // adding scores
        }
    }

    // revert to the previous step of the calculator
    self.PreviousStep = function () {
        if (self.WizardModel().Template == "sevenwonders-new-player-template") {
            // adding players
        }
        else {
            // adding scores
        }
    }
};

var WizardModel = function (template, model) {
    this.Template = template;
    this.Model = model;
};

var PlayerWizardModel = function (playermodel, seat) {
    this.Player = playermodel;
    this.Seat = seat;
};

var ScoreWizardModel = function () {

};

var debug;
$(document).ready(function () {
    var model = new ViewModel();
    ko.applyBindings(mode);
    debug = model;

    $.ajax({
        url: '/SevenWonders/GetNewGameData',
        dataType: 'json',
        success: function (data) {
            if (data.Success) {
                model.Users($.map(data.Payload.users, function (u) {
                    return {
                        id: u.UserId,
                        name: u.UserName
                    };
                }));
                model.Boards($.map(data.Payload.boards, function (u) {
                    return {
                        id: u.BoardId,
                        name: u.Name
                    };
                }));
            }
        }
    });
});