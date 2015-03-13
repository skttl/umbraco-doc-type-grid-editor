angular.module('umbraco.resources').factory('Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources',
    function ($q, $http, umbRequestHelper) {
        return {
            getContentAliasByGuid: function (guid) {
                var url = "/umbraco/backoffice/DocTypeGridEditorApi/DocTypeGridEditorApi/GetContentTypeAliasByGuid?guid=" + guid;
                return umbRequestHelper.resourcePromise(
                    $http.get(url),
                    'Failed to retrieve content type alias by guid'
                );
            },
            getContentTypes: function (allowedContentTypes) {
                var url = "/umbraco/backoffice/DocTypeGridEditorApi/DocTypeGridEditorApi/GetContentTypes";
                if (allowedContentTypes) {
                    for (var i = 0; i < allowedContentTypes.length; i++) {
                        url += (i == 0 ? "?" : "&") + "allowedContentTypes=" + allowedContentTypes[i];
                    }
                }
                return umbRequestHelper.resourcePromise(
                    $http.get(url),
                    'Failed to retrieve content types'
                );
            },
            getContentTypeIcon: function (guid) {
                var url = "/umbraco/backoffice/DocTypeGridEditorApi/DocTypeGridEditorApi/GetContentTypeIcon?guid=" + guid;
                return umbRequestHelper.resourcePromise(
                    $http.get(url),
                    'Failed to retrieve content type icon'
                );
            },
            getDataTypePreValues: function (dtdId) {
                var url = "/umbraco/backoffice/DocTypeGridEditorApi/DocTypeGridEditorApi/GetDataTypePreValues?dtdid=" + dtdId;
                return umbRequestHelper.resourcePromise(
                    $http.get(url),
                    'Failed to retrieve datatypes'
                );
            },
            getEditorMarkupForDocTypePartial: function (nodeId, id, docType, value, viewPath) {
                var url = "/" + nodeId +"?dtgePreview=1";
                return $http({
                    method: 'POST',
                    url: url,
                    data: $.param({
                        id: id,
                        docType: docType,
                        value: JSON.stringify(value),
                        viewPath: viewPath
                    }),
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                });
            }
        };
    });