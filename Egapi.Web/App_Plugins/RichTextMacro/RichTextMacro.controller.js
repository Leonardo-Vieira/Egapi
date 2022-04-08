angular.module("umbraco")
.controller("Macro.RichTextMacro",
    function ($scope) {
    $scope.textInput = {
        label: 'bodyText',
        description: '...',
        view: 'rte',
        value: $scope.model.value,
        config: {
            editor: {
                toolbar: ["code", "removeformat", "link", "unlink", "bold", "italic", "underline", "strikethrough"],
                stylesheets: [],
                dimensions: {
                    height: 200
                }
            }
        }
    };
    $scope.$watch('textInput.value', function (newValue, oldValue) {
        $scope.model.value = newValue;
    });
});