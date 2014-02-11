/*global $, ASHLYNN */

ASHLYNN.namespace("common");

ASHLYNN.common = (function () {
    "use strict";

    var url,
        getData;

    url = function (relativeUrl) {
        return ASHLYNN.baseUrl + relativeUrl;
    };

    getData = function ($element, name) {
        var data = $element.attr('data-ashlynn-' + name);
        return data;
    };

    return {
        url: url,
        getData: getData
    };
}());

