angular.module("umbraco").controller("Our.Umbraco.DocTypeGridEditor.GridEditors.DocTypeGridEditor", [

    "$scope",
    "$timeout",
    "editorState",
    'assetsService',
    "Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources",
    "umbRequestHelper",
    "localizationService",
    "editorService",

    function ($scope, $timeout, editorState, assetsService, dtgeResources, umbRequestHelper, localizationService, editorService) {

        const defaultTitle = "Click to insert item",
            defaultSelectContentTypeLabel = "Choose a Content Type",
            defaultOverlayTitle = "Edit item";

        $scope.title = defaultTitle;
        $scope.selectContentTypeLabel = defaultSelectContentTypeLabel;

        var overlayTitle = defaultOverlayTitle;

        var overlayOptions = {
            view: umbRequestHelper.convertVirtualToAbsolutePath(
                "~/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.dialog.html"),
            model: {}
        };

        $scope.icon = "icon-item-arrangement";

        // localize strings
        localizationService.localizeMany(["docTypeGridEditor_insertItem", "docTypeGridEditor_editItem", "docTypeGridEditor_selectContentType"]).then(function (data) {
            if ($scope.title === defaultTitle) $scope.title = data[0];
            if (overlayTitle === defaultOverlayTitle) overlayTitle = data[1];
            if ($scope.selectContentTypeLabel === defaultSelectContentTypeLabel) $scope.selectContentTypeLabel = data[2];
        });

        $scope.setValue = function (data, callback) {
            $scope.control.value = data;
            if (!("id" in $scope.control.value) || $scope.control.value.id == "") {
                $scope.control.value.id = guid();
            }
            if ("name" in $scope.control.value.value && $scope.control.value.value.name) {
                $scope.title = $scope.control.value.value.name;
            }
            if ("dtgeContentTypeAlias" in $scope.control.value && $scope.control.value.dtgeContentTypeAlias) {
                dtgeResources.getContentTypeIcon($scope.control.value.dtgeContentTypeAlias).then(function (data2) {
                    if (data2.icon) {
                        $scope.icon = data2.icon;
                    }
                });
            }
            if (callback)
                callback();
        };

        $scope.setDocType = function () {

            overlayOptions.title = overlayTitle;
            overlayOptions.submitButtonLabelKey = "bulk_done";
            overlayOptions.editorName = $scope.control.editor.name;
            overlayOptions.allowedDocTypes = $scope.control.editor.config.allowedDocTypes || [];
            overlayOptions.nameTemplate = $scope.control.editor.config.nameTemplate;
            overlayOptions.size = $scope.control.editor.config.largeDialog ? null : "small";

            overlayOptions.dialogData = {
                docTypeAlias: $scope.control.value.dtgeContentTypeAlias,
                value: $scope.control.value.value,
                id: $scope.control.value.id
            };
            overlayOptions.close = function () {
                editorService.close();
            }
            overlayOptions.submit = function (newModel) {

                // Copy property values to scope model value
                if (newModel.node) {
                    var value = {
                        name: newModel.editorName
                    };

                    for (var v = 0; v < newModel.node.variants.length; v++) {
                        var variant = newModel.node.variants[v];
                        for (var t = 0; t < variant.tabs.length; t++) {
                            var tab = variant.tabs[t];
                            for (var p = 0; p < tab.properties.length; p++) {
                                var prop = tab.properties[p];
                                if (typeof prop.value !== "function") {
                                    value[prop.alias] = prop.value;
                                }
                            }
                        }
                    }

                    if (newModel.nameExp) {
                        var newName = newModel.nameExp(value); // Run it against the stored dictionary value, NOT the node object
                        if (newName && (newName = $.trim(newName))) {
                            value.name = newName;
                        }
                    }

                    newModel.dialogData.value = value;
                } else {
                    newModel.dialogData.value = null;

                }

                $scope.setValue({
                    dtgeContentTypeAlias: newModel.dialogData.docTypeAlias,
                    value: newModel.dialogData.value,
                    id: newModel.dialogData.id
                });
                $scope.setPreview($scope.control.value);
                editorService.close();
            };

            editorService.open(overlayOptions);
        };

        $scope.setPreview = function (model) {
            if ($scope.control.editor.config && "enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
                dtgeResources.getEditorMarkupForDocTypePartial(editorState.current.id, model.id,
                    $scope.control.editor.alias, model.dtgeContentTypeAlias, model.value,
                    $scope.control.editor.config.viewPath,
                    $scope.control.editor.config.previewViewPath,
                    !!editorState.current.publishDate)
                    .then(function (response) {
                        var htmlResult = response.data;
                        if (htmlResult.trim().length > 0) {
                            $scope.preview = htmlResult;
                        }
                    });
            }
        };

        function init() {
            $timeout(function () {
                if ($scope.control.$initializing) {
                    $scope.setDocType();
                } else if ($scope.control.value) {
                    $scope.setPreview($scope.control.value);
                }
            }, 200);
        }

        if ($scope.control.value) {
            if (!$scope.control.value.dtgeContentTypeAlias && $scope.control.value.docType) {
                $scope.control.value.dtgeContentTypeAlias = $scope.control.value.docType;
            }
            if ($scope.control.value.docType) {
                delete $scope.control.value.docType;
            }
            if (isGuid($scope.control.value.dtgeContentTypeAlias)) {
                dtgeResources.getContentTypeAliasByGuid($scope.control.value.dtgeContentTypeAlias).then(function (data1) {
                    $scope.control.value.dtgeContentTypeAlias = data1.alias;
                    $scope.setValue($scope.control.value, init);
                });
            } else {
                $scope.setValue($scope.control.value, init);
            }
        } else {
            $scope.setValue({
                id: guid(),
                dtgeContentTypeAlias: "",
                value: {}
            }, init);
        }

        // Load preview css / js files
        if ($scope.control.editor.config && "enablePreview" in $scope.control.editor.config && $scope.control.editor.config.enablePreview) {
            if ("previewCssFilePath" in $scope.control.editor.config && $scope.control.editor.config.previewCssFilePath) {
                assetsService.loadCss($scope.control.editor.config.previewCssFilePath, $scope);
            };

            if ("previewJsFilePath" in $scope.control.editor.config && $scope.control.editor.config.previewJsFilePath) {
                assetsService.loadJs($scope.control.editor.config.previewJsFilePath, $scope);
            }
        }

        function guid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                    .toString(16)
                    .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                s4() + '-' + s4() + s4() + s4();
        }

        function isGuid(input) {
            return new RegExp("^[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}$", "i").test(input.toString());
        }

    }
]);

angular.module("umbraco").controller("Our.Umbraco.DocTypeGridEditor.Dialogs.DocTypeGridEditorDialog",
    [
        "$scope",
        "$interpolate",
        "formHelper",
        "contentResource",
        "Our.Umbraco.DocTypeGridEditor.Resources.DocTypeGridEditorResources",
        "Our.Umbraco.DocTypeGridEditor.Services.DocTypeGridEditorUtilityService",

        function ($scope, $interpolate, formHelper, contentResource, dtgeResources, dtgeUtilityService) {

            var vm = this;
            vm.submit = submit;
            vm.close = close;
            vm.loading = true;
            function submit() {
                if ($scope.model.submit) {
                    $scope.$broadcast('formSubmitting', { scope: $scope });
                    $scope.model.submit($scope.model);
                }
            }
            function close() {
                if ($scope.model.close) {
                    $scope.model.close();
                }
            }

            $scope.docTypes = [];
            $scope.dialogMode = null;
            $scope.selectedDocType = null;
            $scope.model.node = null;

            var nameExp = !!$scope.model.nameTemplate
                ? $interpolate($scope.model.nameTemplate)
                : undefined;

            $scope.model.nameExp = nameExp;

            $scope.selectDocType = function (alias) {
                $scope.dialogMode = "edit";
                $scope.model.dialogData.docTypeAlias = alias;
                loadNode();
            };

            function loadNode() {
                vm.loading = true;
                contentResource.getScaffold(-20, $scope.model.dialogData.docTypeAlias).then(function (data) {

                    // Merge current value
                    if ($scope.model.dialogData.value) {
                        for (var v = 0; v < data.variants.length; v++) {
                            var variant = data.variants[v];
                            for (var t = 0; t < variant.tabs.length; t++) {
                                var tab = variant.tabs[t];
                                for (var p = 0; p < tab.properties.length; p++) {
                                    var prop = tab.properties[p];
                                    if ($scope.model.dialogData.value[prop.alias]) {
                                        prop.value = $scope.model.dialogData.value[prop.alias];
                                    }
                                }
                            }
                        }
                    };

                    // Assign the model to scope
                    $scope.nodeContext = $scope.model.node = data;
                    vm.content = $scope.nodeContext.variants[0];
                    vm.loading = false;
                });
            }

            if ($scope.model.dialogData.docTypeAlias) {
                $scope.dialogMode = "edit";
                loadNode();
            } else {
                $scope.dialogMode = "selectDocType";
                // No data type, so load a list to choose from
                dtgeResources.getContentTypes($scope.model.allowedDocTypes).then(function (docTypes) {
                    $scope.docTypes = docTypes;
                    if ($scope.docTypes.length == 1) {
                        $scope.model.dialogData.docTypeAlias = $scope.docTypes[0].alias;
                        $scope.dialogMode = "edit";
                        loadNode();
                    }
                    else {
                        vm.loading = false;
                    }
                });
            }

        }

    ]);
