import React from "react";

function ErrorMessage({ message }) {
  return (
    <div className="text-red-600 bg-red-50 border border-red-200 rounded px-4 py-2 my-4">
      <strong>Error:</strong> {message}
    </div>
  );
}

export default ErrorMessage;
