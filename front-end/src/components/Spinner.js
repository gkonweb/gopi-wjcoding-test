import React from "react";

function Spinner() {
  return (
    <div style={{ textAlign: "center", margin: "20px 0" }}>
      <div className="spinner" style={{
        width: 40, height: 40, border: "4px solid #ccc", borderTop: "4px solid #333",
        borderRadius: "50%", animation: "spin 1s linear infinite", margin: "0 auto"
      }} />
      <style>
        {`@keyframes spin { 100% { transform: rotate(360deg); } }`}
      </style>
    </div>
  );
}

export default Spinner;
