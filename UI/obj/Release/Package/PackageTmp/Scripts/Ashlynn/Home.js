/*global $, ASHLYNN */

ASHLYNN.namespace("home");

ASHLYNN.home = (function () {
	"use strict";

    var selector,
        url,
        showList,
        search,
        loadRecent,
        deleteEntry,
	    ready;

    selector = {
        addEntryButton: "#add-entry-button",
        searchButton: "#search-button",
        searchResultDiv: "#search-result-div",
        searchBox: "#search-textbox",
        deleteButton: "._delete-button",
        editButton: "._edit-button",
        searchResultItemDiv: "._search-result-item-div",
        recentButton: "#recent-button",
        binButton: "#bin-button"
    };
    
    url = {
        search: ASHLYNN.common.url("/Home/Search"),
        deleteEntry: ASHLYNN.common.url("/Home/MoveToBin"),
        loadRecent: ASHLYNN.common.url("/Home/LoadRecent")
    };

    showList = function (data) {
        var $resultsDiv = $(selector.searchResultDiv);
        $resultsDiv.html(data);
        $(selector.deleteButton, $resultsDiv).button();
        $(selector.editButton, $resultsDiv).button();
    };
    
    search = function () {
        ASHLYNN.ajax.post({
            url: url.search,
            success: showList,
            data: {
                query: $(selector.searchBox).val()
            }
        });
    };

    loadRecent = function () {
        ASHLYNN.ajax.post({
            url: url.loadRecent,
            success: showList
        });
    };

    deleteEntry = function () {
        var $this = $(this),
            $div = $this.closest(selector.searchResultItemDiv),
            id = ASHLYNN.common.getData($div, 'id');
        
        ASHLYNN.ajax.post({
            url: url.deleteEntry,
            success: function () {
                $div.remove();
            },
            data: {
                id: id
            }
        });
    };
    
    ready = function () {
        var $resultsDiv = $(selector.searchResultDiv);
        $(selector.addEntryButton).button();
        $(selector.searchButton).button().click(search);
        $resultsDiv.on("click", selector.deleteButton, deleteEntry);
        $(selector.deleteButton, $resultsDiv).button();
        $(selector.editButton, $resultsDiv).button();
        $(selector.recentButton).button().click(loadRecent);
        $(selector.binButton).button();
    };

	return {
		ready: ready
	};
}());