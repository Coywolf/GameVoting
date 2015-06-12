var GameModel = function (data) {
    data = data || {};
    var self = this;

    self.GameId = data.GameId;
    self.CreatedDate = data.CreatedDate;

    self.Players = ko.observableArray(data.Players == undefined ? null :
        $.map(data.Players, function (p) {
            return new PlayerModel(p);
        })
    );

    // returns a list of players that has at least 7 items, to make the score sheet look nicer
    self.PaddedPlayers = ko.pureComputed(function () {
        var players = self.Players().slice(0);
        for (var i = 0; players.length < 7; i++) {
            players.push(new PlayerModel());
        }
        return players;
    });

    // existing games use the readonly template
    self.cellTemplate = data.GameId == undefined ?
        "sevenwonders-scoresheet-inputcell-template" :
        "sevenwonders-scoresheet-readonlycell-template";

    self.url = ko.pureComputed(function () {
        return "/SevenWonders/Game/" + self.GameId;
    });
}

var PlayerModel = function (data) {
    data = data || {};
    var self = this;

    self.UserId = ko.observable(data.UserId);
    self.BoardId = ko.observable(data.BoardId);
    
    self.Name = ko.observable(data.Name);
    self.BoardName = ko.observable(data.BoardName);
    
    self.MilitaryScore = ko.observable(data.MilitaryScore);
    self.CoinScore = ko.observable(data.CoinScore);
    self.WonderScore = ko.observable(data.WonderScore);
    self.CivicScore = ko.observable(data.CivicScore);
    self.CommercialScore = ko.observable(data.CommercialScore);
    self.GuildScore = ko.observable(data.GuildScore);
    self.ScienceScore = ko.observable(data.ScienceScore);
    self.LeaderScore = ko.observable(data.LeaderScore);

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