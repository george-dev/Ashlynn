/*global $ */

var ASHLYNN = ASHLYNN || {};

ASHLYNN.namespace = function (namespaceString) {
	"use strict";

	var parts = namespaceString.split("."),
		parent = ASHLYNN,
		i;

	if (parts[0] === "ASHLYNN") {
		parts = parts.slice(1);
	}

	for (i = 0; i < parts.length; i += 1) {
		if (parent[parts[i]] === undefined) {
			parent[parts[i]] = {};
		}
		parent = parent[parts[i]];
	}
	return parent;
};