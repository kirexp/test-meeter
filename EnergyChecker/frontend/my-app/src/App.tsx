import React, { useState } from 'react';

export default function MeterReadingUpload() {
  const [file, setFile] = useState<File | null>(null);
  const [result, setResult] = useState<{ processed: number; failed: number } | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.length) {
      setFile(e.target.files[0]);
      setResult(null);
      setError(null);
    }
  };

  const handleUpload = async () => {
    if (!file) return;

    const formData = new FormData();
    formData.append('file', file);

    setLoading(true);
    setResult(null);
    setError(null);

    try {
      const response = await fetch('https://localhost:7215/meter-reading-uploads', {
        method: 'POST',
        body: formData
      });

      if (!response.ok) {
        throw new Error(`Upload failed with status ${response.status}`);
      }

      const data = await response.json(); // expects { processed: N, failed: M }
      setResult(data);
    } catch (err: any) {
      setError(err.message ?? 'Unknown error occurred');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 max-w-md mx-auto bg-white rounded shadow">
      <h2 className="text-xl font-bold mb-4">Upload Meter Readings CSV</h2>

      <input
        type="file"
        accept=".csv"
        onChange={handleFileChange}
        className="mb-2"
      />

      <button
        disabled={!file || loading}
        onClick={handleUpload}
        className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
      >
        {loading ? 'Uploading...' : 'Upload'}
      </button>

      {result && (
        <div className="mt-4 text-green-600">
          ✅ Processed: {result.processed}, ❌ Failed: {result.failed}
        </div>
      )}

      {error && <div className="mt-4 text-red-600">⚠️ {error}</div>}
    </div>
  );
}