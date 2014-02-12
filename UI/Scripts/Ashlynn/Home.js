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
		showTextInPopup,
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
		binButton: "#bin-button",
		nameLink: "._name-link",
		dialog: "#dialog",
		textarea: "textarea"
	};

	url = {
		search: ASHLYNN.common.url("/Home/Search"),
		deleteEntry: ASHLYNN.common.url("/Home/MoveToBin"),
		loadRecent: ASHLYNN.common.url("/Home/LoadRecent"),
		displayText: ASHLYNN.common.url("/Home/DisplayText")
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

	showTextInPopup = function () {
		var $this = $(this),
			$div = $this.closest(selector.searchResultItemDiv),
			id = ASHLYNN.common.getData($div, 'id');

		ASHLYNN.ajax.post({
			url: url.displayText,
			success: function (data) {
				var $dialog = $(selector.dialog);
				$(selector.textarea, $dialog).empty().append(data);
				$dialog.dialog("open");
			},
			data: {
				id: id
			}
		});
	};

	ready = function () {
	    var $resultsDiv = $(selector.searchResultDiv),
	        dialogWidth,
	        dialogHeight;
	    
		$(selector.addEntryButton).button();
		$(selector.searchButton).button().click(search);
		$resultsDiv.on("click", selector.deleteButton, deleteEntry);
		$resultsDiv.on("click", selector.nameLink, showTextInPopup);
		$(selector.deleteButton, $resultsDiv).button();
		$(selector.editButton, $resultsDiv).button();
		$(selector.recentButton).button().click(loadRecent);
		$(selector.binButton).button();
		dialogWidth = $(window).width() * 0.8;
	    dialogHeight = $(window).height() * 0.8;
	    $(selector.dialog).dialog({ autoOpen: false, width: dialogWidth, height: dialogHeight });
	};

	return {
		ready: ready
	};
}());