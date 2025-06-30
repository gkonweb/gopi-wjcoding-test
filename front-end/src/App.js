import React, { useEffect, useState } from "react";
import MovieList from "./components/MovieList";
import MovieModal from "./components/MovieModal";
import Spinner from "./components/Spinner";
import ErrorMessage from "./components/ErrorMessage";

function App() {
  const [movies, setMovies] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedPriceId, setSelectedPriceId] = useState(null);

  useEffect(() => {
    setLoading(true);
    fetch(`${process.env.REACT_APP_API_URL}/Movies/list`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch movies.");
        return res.json();
      })
      .then(setMovies)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  const handlePriceClick = (priceId) => {
    setSelectedPriceId(priceId);
    setModalOpen(true);
  };

  return (
    <div className="min-h-screen bg-gray-50 px-4 py-8">
      <div className="max-w-7xl mx-auto bg-white shadow-lg rounded-xl p-8">
        <h1 className="text-3xl font-bold mb-8 text-center text-blue-700">
          Movie Listings
        </h1>
        {loading && <Spinner />}
        {error && <ErrorMessage message={error} />}
        {!loading && !error && (
          <div className="">
            <MovieList movies={movies} onPriceClick={handlePriceClick} />
          </div>
        )}
        <MovieModal
          open={modalOpen}
          priceId={selectedPriceId}
          onClose={() => setModalOpen(false)}
        />
      </div>
    </div>
  );
}

export default App;
