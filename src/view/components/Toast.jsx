import React from 'react';
import { motion, AnimatePresence } from 'framer-motion';

export default function Toast({ message, show }) {
  return (
    <div className="fixed right-4 bottom-4 z-50">
      <AnimatePresence>
        {show && (
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: 10 }} className="bg-black text-white px-4 py-2 rounded">
            {message}
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}
