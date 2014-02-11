/*global $, ASHLYNN */

ASHLYNN.namespace("ajax");

ASHLYNN.ajax = (function() {
	"use strict";

	var post;

	post = function(options) {
		var postOptions = {
			type: "POST",
			contentType: "application/json; charset=utf-8",
			url: options.url,
			data: JSON.stringify(options.data),
			success: options.success
		};

		$.ajax(postOptions);
	};

	return {
		post: post
	};
}());