/*global $, ASHLYNN */

ASHLYNN.namespace("recycleBin");

ASHLYNN.recycleBin = (function () {
	"use strict";

    var selector,
        url,
        removeEntry,
	    ready;

    selector = {
        deleteButton: "._delete-button",
        restoreButton: "._restore-button",
        homeButton: "#home-button",
        binDiv: "._bin-div"
    };
    
    url = {
        deleteEntry: ASHLYNN.common.url("/RecycleBin/Delete"),
        restoreEntry: ASHLYNN.common.url("/RecycleBin/Restore")
    };

    removeEntry = function ($button, removeUrl) {
        var $div = $button.closest(selector.binDiv),
            id = ASHLYNN.common.getData($div, 'id');

        ASHLYNN.ajax.post({
            url: removeUrl,
            success: function () {
                $div.remove();
            },
            data: {
                id: id
            }
        });
    };
    
    ready = function () {
        var $binDiv = $(selector.binDiv);
        $binDiv.on("click", selector.deleteButton, function () {
            removeEntry($(this), url.deleteEntry);
        });
        $binDiv.on("click", selector.restoreButton, function () {
            removeEntry($(this), url.restoreEntry);
        });
        $(selector.deleteButton, $binDiv).button();
        $(selector.restoreButton, $binDiv).button();
        $(selector.homeButton).button();
    };

	return {
		ready: ready
	};
}());