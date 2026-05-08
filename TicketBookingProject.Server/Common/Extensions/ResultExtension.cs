using Microsoft.AspNetCore.Mvc;

namespace TicketBookingProject.Server.Common.Extensions
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T> (this Result<T> result)
        {
            if (result.StatusCode == StatusCodes.Status204NoContent)
            {
                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }

            var response = new NApiResponse<T>
            {
                Success = result.IsSuccess,
                Message = result.Message,
                Data = result.Data,
                Errors = result.Errors
            };

            return new ObjectResult(response)
            {
                StatusCode = result.StatusCode,
            };
        }
    }
}
