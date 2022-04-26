var resultFormatNumber = function (value) {
    return new Promise(function (resolve, reject) {
        $.when(
            $.getJSON("/Scripts/cldr/supplemental/likelySubtags.json"),
            $.getJSON("/Scripts/cldr/main/en/numbers.json"),
            $.getJSON("/Scripts/cldr/supplemental/numberingSystems.json"),
            $.getJSON("/Scripts/cldr/main/en/ca-gregorian.json"),
            $.getJSON("/Scripts/cldr/main/en/timeZoneNames.json"),
            $.getJSON("/Scripts/cldr/supplemental/timeData.json"),
            $.getJSON("/Scripts/cldr/supplemental/weekData.json")
        ).then(function () {

            // Normalize $.get results, we only need the JSON, not the request statuses.
            return [].slice.apply(arguments, [0]).map(function (result) {
                return result[0];
            });

        }).then(Globalize.load).then(function () {
            Globalize.locale("en");
            formatter = Globalize.numberFormatter({
                maximumFractionDigits: 2,
                minimumFractionDigits: 2
            });
            resolve (formatter(Globalize("en").parseNumber(value)));
        });
    })
}

