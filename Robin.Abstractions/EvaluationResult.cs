using Robin.Abstractions.Facades;

namespace Robin.Abstractions;




public record EvaluationResult(ResoltionState Status, IDataFacade Value);
