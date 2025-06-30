import React, { useEffect, useState } from "react";
import Spinner from "./Spinner";
import ErrorMessage from "./ErrorMessage";
import ImgWithFallback from "./ImgWithFallback";

function MovieModal({ open, priceId, onClose }) {
  const [details, setDetails] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (open && priceId) {
      setLoading(true);
      setError("");
      setDetails(null);
      fetch(`${process.env.REACT_APP_API_URL}/Movies/${priceId}`)
        .then((res) => {
          if (!res.ok) throw new Error("Failed to fetch movie details.");
          return res.json();
        })
        .then(setDetails)
        .catch((err) => setError(err.message))
        .finally(() => setLoading(false));
    }
  }, [open, priceId]);

  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-2xl p-8 min-w-[320px] max-w-lg w-full relative">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-gray-400 hover:text-gray-700 text-lg font-bold"
          aria-label="Close"
        >
          ×
        </button>
        {loading && <Spinner />}
        {error && <ErrorMessage message={error} />}
        {details && (
          <div>
            <h2 className="text-2xl font-bold mb-2 text-blue-800">{details.title}</h2>
            <ImgWithFallback
              src={details.poster}
              alt={details.title}
              className="w-36 h-52 object-cover rounded mb-4 shadow"
            />
            <p className="mb-1"><strong>Year:</strong> {details.year}</p>
            <p className="mb-1"><strong>Type:</strong> {details.type}</p>
            <p className="mb-1"><strong>Plot:</strong> {details.plot}</p>
            {/* Add more fields as needed */}
          </div>
        )}
      </div>
    </div>
  );
}

export default MovieModal;
