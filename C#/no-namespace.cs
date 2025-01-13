public class NLPUnit {
    private static class Token {
        private String text;
        private String pos;  // Part of speech
        private String lemma;
        
        public Token(String text, String pos, String lemma) {
            this.text = text;
            this.pos = pos;
            this.lemma = lemma;
        }
        
        @Override
        public String toString() {
            return String.format("Token{text='%s', pos='%s', lemma='%s'}", text, pos, lemma);
        }
    }
    
    private static class Sentence {
        private List<Token> tokens;
        private String originalText;
        
        public Sentence(String text, List<Token> tokens) {
            this.originalText = text;
            this.tokens = tokens;
        }
        
        public List<Token> getTokens() {
            return new ArrayList<>(tokens);
        }
        
        @Override
        public String toString() {
            return String.format("Sentence{text='%s', tokenCount=%d}", originalText, tokens.size());
        }
    }

    // Basic tokenization using regex
    public List<String> tokenize(String text) {
        // Split on whitespace and punctuation
        return Arrays.asList(text.split("\\s+|(?=\\p{Punct})|(?<=\\p{Punct})"))
                    .stream()
                    .filter(token -> !token.trim().isEmpty())
                    .collect(Collectors.toList());
    }

    // Simple sentence segmentation
    public List<String> segmentSentences(String text) {
        // Split on common sentence endings followed by whitespace and capital letter
        return Arrays.asList(text.split("(?<=[.!?])\\s+(?=[A-Z])"));
    }

    // Basic stemming implementation (Porter-like algorithm)
    public String stem(String word) {
        String lowercaseWord = word.toLowerCase();
        
        // Handle basic plural forms
        if (lowercaseWord.endsWith("ies")) {
            return lowercaseWord.substring(0, lowercaseWord.length() - 3) + "y";
        }
        if (lowercaseWord.endsWith("es")) {
            return lowercaseWord.substring(0, lowercaseWord.length() - 2);
        }
        if (lowercaseWord.endsWith("s") && !lowercaseWord.endsWith("ss")) {
            return lowercaseWord.substring(0, lowercaseWord.length() - 1);
        }
        
        // Handle -ing forms
        if (lowercaseWord.endsWith("ing")) {
            String stem = lowercaseWord.substring(0, lowercaseWord.length() - 3);
            if (stem.length() > 0) {
                return stem;
            }
        }
        
        // Handle -ed forms
        if (lowercaseWord.endsWith("ed")) {
            String stem = lowercaseWord.substring(0, lowercaseWord.length() - 2);
            if (stem.length() > 0) {
                return stem;
            }
        }
        
        return lowercaseWord;
    }

    // Remove common stop words
    private static final Set<String> STOP_WORDS = new HashSet<>(Arrays.asList(
        "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
        "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
        "to", "was", "were", "will", "with"
    ));

    public List<String> removeStopWords(List<String> tokens) {
        return tokens.stream()
                    .filter(token -> !STOP_WORDS.contains(token.toLowerCase()))
                    .collect(Collectors.toList());
    }

    // Basic part-of-speech tagging
    private static final Map<String, String> SIMPLE_POS_RULES = new HashMap<>();
    static {
        SIMPLE_POS_RULES.put("the", "DET");
        SIMPLE_POS_RULES.put("a", "DET");
        SIMPLE_POS_RULES.put("an", "DET");
        SIMPLE_POS_RULES.put("is", "VERB");
        SIMPLE_POS_RULES.put("are", "VERB");
        SIMPLE_POS_RULES.put("was", "VERB");
        SIMPLE_POS_RULES.put("were", "VERB");
    }

    public String getPOS(String word) {
        String lowercaseWord = word.toLowerCase();
        
        // Check predefined rules
        if (SIMPLE_POS_RULES.containsKey(lowercaseWord)) {
            return SIMPLE_POS_RULES.get(lowercaseWord);
        }
        
        // Simple heuristics
        if (word.endsWith("ly")) {
            return "ADV";  // Adverb
        }
        if (word.endsWith("ing")) {
            return "VERB";  // Gerund or present participle
        }
        if (word.endsWith("ed")) {
            return "VERB";  // Past tense
        }
        if (word.endsWith("s")) {
            return "NOUN";  // Plural noun
        }
        
        // Default to noun
        return "NOUN";
    }

    // Named Entity Recognition (very basic implementation)
    private static final Set<String> PERSON_TITLES = new HashSet<>(Arrays.asList(
        "mr", "mrs", "ms", "dr", "prof"
    ));

    public boolean isNamedEntity(String token) {
        String lowercaseToken = token.toLowerCase();
        
        // Check for person titles
        if (PERSON_TITLES.contains(lowercaseToken)) {
            return true;
        }
        
        // Check for capitalized words (potential proper nouns)
        if (Character.isUpperCase(token.charAt(0))) {
            return true;
        }
        
        return false;
    }

    // Text similarity using cosine similarity of word frequencies
    public double calculateSimilarity(String text1, String text2) {
        // Tokenize and get word frequencies
        Map<String, Integer> freq1 = getWordFrequencies(text1);
        Map<String, Integer> freq2 = getWordFrequencies(text2);
        
        // Get all unique words
        Set<String> allWords = new HashSet<>();
        allWords.addAll(freq1.keySet());
        allWords.addAll(freq2.keySet());
        
        // Calculate dot product and magnitudes
        double dotProduct = 0.0;
        double magnitude1 = 0.0;
        double magnitude2 = 0.0;
        
        for (String word : allWords) {
            int count1 = freq1.getOrDefault(word, 0);
            int count2 = freq2.getOrDefault(word, 0);
            
            dotProduct += count1 * count2;
            magnitude1 += count1 * count1;
            magnitude2 += count2 * count2;
        }
        
        magnitude1 = Math.sqrt(magnitude1);
        magnitude2 = Math.sqrt(magnitude2);
        
        if (magnitude1 == 0.0 || magnitude2 == 0.0) {
            return 0.0;
        }
        
        return dotProduct / (magnitude1 * magnitude2);
    }

    private Map<String, Integer> getWordFrequencies(String text) {
        Map<String, Integer> frequencies = new HashMap<>();
        List<String> tokens = tokenize(text.toLowerCase());
        
        for (String token : tokens) {
            frequencies.put(token, frequencies.getOrDefault(token, 0) + 1);
        }
        
        return frequencies;
    }

    // Process a complete text
    public List<Sentence> processText(String text) {
        List<Sentence> sentences = new ArrayList<>();
        
        // Segment into sentences
        List<String> sentenceTexts = segmentSentences(text);
        
        for (String sentenceText : sentenceTexts) {
            List<String> tokens = tokenize(sentenceText);
            List<Token> processedTokens = new ArrayList<>();
            
            for (String tokenText : tokens) {
                String pos = getPOS(tokenText);
                String lemma = stem(tokenText);
                processedTokens.add(new Token(tokenText, pos, lemma));
            }
            
            sentences.add(new Sentence(sentenceText, processedTokens));
        }
        
        return sentences;
    }

    // Example usage
    public static void main(String[] args) {
        NLPUnit nlp = new NLPUnit();
        String text = "The quick brown fox jumps over the lazy dog. It was a sunny day.";
        
        // Process complete text
        List<Sentence> sentences = nlp.processText(text);
        for (Sentence sentence : sentences) {
            System.out.println("\nSentence: " + sentence.originalText);
            for (Token token : sentence.getTokens()) {
                System.out.println(token);
            }
        }
        
        // Demonstrate similarity
        String text1 = "The cat is on the mat";
        String text2 = "The dog is on the mat";
        double similarity = nlp.calculateSimilarity(text1, text2);
        System.out.printf("\nSimilarity between texts: %.2f\n", similarity);
    }
}
